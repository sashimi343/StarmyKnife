using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins
{
    public interface IPluginParameter
    {
        string Key { get; }
        string Name { get; }

        T GetValue<T>();
        void SetValue(object value);
    }
}
