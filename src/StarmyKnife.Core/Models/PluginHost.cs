using StarmyKnife.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Models
{
    public class PluginHost
    {
        public IPlugin Plugin { get; }
        public IPluginMetadata Metadata { get; }
        public PluginParameterCollection Parameters { get; }

        public string Name => Metadata.Name;
        public string HelpText => Metadata.HelpText;
        public bool HasHelpText => !string.IsNullOrWhiteSpace(Metadata.HelpText);

        internal PluginHost(IPlugin plugin, IPluginMetadata metadata)
        {
            Plugin = plugin;
            Metadata = metadata;
            Parameters = plugin.GetParametersSchema();
        }
    }
}
