using StarmyKnife.Core.Models;
using StarmyKnife.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace StarmyKnife.Core.Contracts.Services
{
    public interface IPluginLoaderService
    {
        bool UsePrettyValidatorAsConverter { get; set; }

        void LoadPlugins(Assembly assembly);
        List<PluginHost> GetPlugins<T>() where T : IPlugin;
    }
}
