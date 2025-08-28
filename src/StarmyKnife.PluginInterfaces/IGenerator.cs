using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.PluginInterfaces
{
    public interface IGenerator : IPlugin
    {
        string Generate(PluginParameterCollection parameters);
    }
}
