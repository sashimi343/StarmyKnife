using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins.Internal
{
    public sealed class FlagPluginParameter : IPluginParameter
    {
        private readonly string _key;
        private readonly string _name;
        private bool _value;

        public FlagPluginParameter(string key, string name, bool defaultValue)
        {
            _key = key;
            _name = name;
            _value = defaultValue;
        }

        public string Key => _key;

        public string Name => _name;

        public bool Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public T GetValue<T>()
        {
            if (typeof(T) != typeof(bool))
            {
                throw new InvalidOperationException("Cannot convert bool to " + typeof(T).Name);
            }

            return (T)(object)_value;
        }

        public void SetValue(object value)
        {
            if (value is bool boolValue)
            {
                _value = boolValue;
            }
            else
            {
                throw new ArgumentException("Value must be a bool");
            }
        }
    }
}
