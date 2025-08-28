using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.PluginInterfaces
{
    public interface IConverter : IPlugin
    {
        PluginInvocationResult Convert(string input, PluginParameterCollection parameters);
    }
}
