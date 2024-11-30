using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins
{
    public interface IPrettyValidator : IPlugin
    {
        bool CanPrettify { get; }
        bool CanMinify { get; }
        ValidationResult Validate(string input, PluginParameterCollection parameters);
        PluginInvocationResult Prettify(string input, PluginParameterCollection parameters);
        PluginInvocationResult Minify(string input, PluginParameterCollection parameters);
    }
}
