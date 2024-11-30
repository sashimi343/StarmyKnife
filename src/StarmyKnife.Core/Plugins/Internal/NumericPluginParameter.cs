using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins.Internal
{
    public class NumericPluginParameter : IPluginParameter
    {
        private readonly string _key;
        private readonly string _name;
        private decimal _value;
        private readonly bool _isInteger;

        public string Key => _key;
        public string Name => _name;
        public bool IsInteger => _isInteger;
        public decimal Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_isInteger && (int)value != value)
                {
                    throw new ArgumentException("Value must be an integer");
                }

                _value = value;
            }
        }

        public NumericPluginParameter(string key, string name, bool isInteger, decimal defaultValue)
        {
            _key = key;
            _name = name;
            _value = defaultValue;
            _isInteger = isInteger;
        }

        public T GetValue<T>()
        {
            if (_isInteger && !IsIntegerType(typeof(T)))
            {
                throw new InvalidOperationException("Cannot convert integer to " + typeof(T).Name);
            }
            else if (!IsNumericType(typeof(T)))
            {
                throw new InvalidOperationException("Cannot convert number to " + typeof(T).Name);
            }

            if (_isInteger)
            {
                return (T)(object)(int)_value;
            }
            else
            {
                return (T)(object)_value;
            }
        }

        public void SetValue(object value)
        {
            if (value is null || !IsNumericType(value.GetType()))
            {
                throw new InvalidOperationException("Cannot convert " + value.GetType().Name + " to number");
            }

            Value = (decimal)value;
        }

        private bool IsIntegerType(Type type)
        {
            return type == typeof(int)
                    || type == typeof(uint)
                    || type == typeof(short)
                    || type == typeof(ushort)
                    || type == typeof(byte)
                    || type == typeof(sbyte)
                    || type == typeof(long)
                    || type == typeof(ulong);
        }

        private bool IsNumericType(Type type)
        {
            return type == typeof(int)
                    || type == typeof(uint)
                    || type == typeof(ushort)
                    || type == typeof(short)
                    || type == typeof(byte)
                    || type == typeof(sbyte)
                    || type == typeof(long)
                    || type == typeof(ulong)
                    || type == typeof(float)
                    || type == typeof(double)
                    || type == typeof(decimal);
        }
    }
}
