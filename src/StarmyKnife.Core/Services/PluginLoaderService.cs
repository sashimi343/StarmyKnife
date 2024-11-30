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

        public List<PluginHost> GetPlugins<T>() where T : IPlugin
        {
            var aggregateCatalog = new AggregateCatalog();

            // Load built-in plugins
            var catalog = new AssemblyCatalog(typeof(IPlugin).Assembly);
            aggregateCatalog.Catalogs.Add(catalog);

            // Load external plugins
            foreach (var assembly in _externalPluginAssemblies)
            {
                catalog = new AssemblyCatalog(assembly);
                aggregateCatalog.Catalogs.Add(catalog);
            }

            var container = new CompositionContainer(aggregateCatalog);
            container.ComposeParts(this);

            var selectedPlugins = Plugins.Where(p => p.Value is T).Select(p => new PluginHost(p.Value, p.Metadata)).ToList();

            return selectedPlugins;
        }

        public void LoadPlugins(Assembly assembly)
        {
            _externalPluginAssemblies.Add(assembly);
        }
    }
}
