using StarmyKnife.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Models
{
    public class PluginHost
    {
        private const string SearchTextNameAndHelpTextSeparator = "\n";

        public IPlugin Plugin { get; }
        public IPluginMetadata Metadata { get; }
        public PluginParameterCollection Parameters { get; }
        public string SearchText { get; }

        public string Name => Metadata.Name;
        public string HelpText => Metadata.HelpText;
        public bool HasHelpText => !string.IsNullOrWhiteSpace(Metadata.HelpText);

        internal PluginHost(IPlugin plugin, IPluginMetadata metadata)
        {
            Plugin = plugin;
            Metadata = metadata;
            Parameters = plugin.GetParametersSchema();
            SearchText = GetSearchText(metadata);
        }

        private string GetSearchText(IPluginMetadata metadata)
        {
            var sb = new StringBuilder();
            sb.Append(SearchTextNormalizer.Normalize(metadata.Name));
            if (!string.IsNullOrWhiteSpace(metadata.HelpText))
            {
                sb.Append(SearchTextNameAndHelpTextSeparator);
                sb.Append(SearchTextNormalizer.Normalize(metadata.HelpText));
            }
            return sb.ToString();
        }
    }
}
