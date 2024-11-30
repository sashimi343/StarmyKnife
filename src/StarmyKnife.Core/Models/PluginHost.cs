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

        internal PluginHost(IPlugin plugin, IPluginMetadata metadata)
        {
            Plugin = plugin;
            Metadata = metadata;
            Parameters = plugin.GetParametersSchema();
        }
    }
}
