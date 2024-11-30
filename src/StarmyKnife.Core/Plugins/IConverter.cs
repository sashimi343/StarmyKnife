using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins
{
    public interface IConverter : IPlugin
    {
        PluginInvocationResult Convert(string input, PluginParameterCollection parameters);
    }
}
