using StarmyKnife.Core.Contracts.Services;
using StarmyKnife.Core.Models;
using StarmyKnife.Core.Plugins;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Text;

namespace StarmyKnife.Core.Services
{
    public class PluginLoaderService : IPluginLoaderService
    {
        [ImportMany]
        private IEnumerable<Lazy<PluginBase, IPluginMetadata>> Plugins { get; set; }

        private readonly List<Assembly> _externalPluginAssemblies;

        public PluginLoaderService()
        {
            _externalPluginAssemblies = [];
        }

        public bool UsePrettyValidatorAsConverter { get; set; } = false;

        public List<PluginHost> GetPlugins<T>(out string errorMessage) where T : IPlugin
        {
            try
            {
                var plugins = GetPluginsInternal<T>(false);
                errorMessage = null;
                return plugins;
            }
            catch (CompositionException ex)
            {
                var pluginsOnlyBuiltIn = GetPluginsInternal<T>(true);
                errorMessage = BuildPluginLoadErrorMessage(ex);
                return pluginsOnlyBuiltIn;
            }
        }

        private List<PluginHost> GetPluginsInternal<T>(bool disableExternalPlugins) where T : IPlugin
        {
            var aggregateCatalog = new AggregateCatalog();

            // Load built-in plugins
            var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            aggregateCatalog.Catalogs.Add(catalog);

            // Load external plugins
            if (!disableExternalPlugins)
            {
                foreach (var assembly in _externalPluginAssemblies)
                {
                    catalog = new AssemblyCatalog(assembly);
                    aggregateCatalog.Catalogs.Add(catalog);
                }
            }

            var container = new CompositionContainer(aggregateCatalog);
            container.ComposeParts(this);

            var selectedPlugins = Plugins.Where(p => p.Value is T).Select(p => new PluginHost(p.Value, p.Metadata)).ToList();

            if (typeof(T) == typeof(IConverter) && UsePrettyValidatorAsConverter)
            {
                selectedPlugins.AddRange(GetConvertersFromPrettyValidators(disableExternalPlugins));
            }

            return selectedPlugins;
        }

        public void LoadPlugins(Assembly assembly)
        {
            _externalPluginAssemblies.Add(assembly);
        }

        private List<PluginHost> GetConvertersFromPrettyValidators(bool disableExternalPlugins)
        {
            var prettyValidators = GetPluginsInternal<IPrettyValidator>(disableExternalPlugins);
            var prettifyConverters = prettyValidators.Where(p => ((IPrettyValidator)p.Plugin).CanPrettify)
                .Select(p => new PrettifierToConverterAdapter((IPrettyValidator)p.Plugin))
                .Select(a => new PluginHost(a, a.Metadata))
                .ToList();
            var minifyConverters = prettyValidators.Where(p => ((IPrettyValidator)p.Plugin).CanMinify)
                .Select(p => new MinifierToConverterAdapter((IPrettyValidator)p.Plugin))
                .Select(a => new PluginHost(a, a.Metadata))
                .ToList();

            return prettifyConverters.Concat(minifyConverters).ToList();
        }

        private string BuildPluginLoadErrorMessage(CompositionException ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine("The external plugins have been disabled due to an error occurring during the loading of the following plugins.");
            foreach (var error in ex.Errors)
            {
                sb.AppendFormat("- {0}", error.Description);
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
