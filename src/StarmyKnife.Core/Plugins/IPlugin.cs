using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins
{
    public interface IPlugin
    {
        PluginParameterCollection GetParametersSchema();
    }
}
