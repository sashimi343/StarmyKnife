using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.PluginInterfaces
{
    public interface IPlugin
    {
        PluginParameterCollection GetParametersSchema();
    }
}
