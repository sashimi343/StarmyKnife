using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins.Internal
{
    public sealed class TextPluginParameter : IPluginParameter
    {
        private readonly string _key;
        private readonly string _name;
        private string _value;

        internal TextPluginParameter(string key, string name, string defaultValue)
        {
            _key = key;
            _name = name;
            _value = defaultValue;
        }

        public string Key => _key;

        public string Name => _name;

        public string Text
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
            if (typeof(T) != typeof(string))
            {
                throw new InvalidOperationException("Cannot convert string to " + typeof(T).Name);
            }

            return (T)(object)_value;
        }

        public void SetValue(object value)
        {
            if (value is string strValue)
            {
                _value = strValue;
            }
            else
            {
                throw new ArgumentException("Value must be a string");
            }
        }
    }
}
