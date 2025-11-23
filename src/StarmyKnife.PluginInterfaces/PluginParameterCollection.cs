using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using StarmyKnife.PluginInterfaces.Internal;

namespace StarmyKnife.PluginInterfaces
{
    public class PluginParameterCollection : IReadOnlyDictionary<string, IPluginParameter>
    {
        private readonly List<KeyValuePair<string, IPluginParameter>> _parameters;

        internal PluginParameterCollection()
        {
            _parameters = new List<KeyValuePair<string, IPluginParameter>>();
        }

        public IPluginParameter this[string key]
        {
            get
            {
                if (TryGetValue(key, out IPluginParameter value))
                {
                    return value;
                }
                throw new KeyNotFoundException();
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return _parameters.Select(_parameters => _parameters.Key);
            }
        }

        public IEnumerable<IPluginParameter> Values
        {
            get
            {
                return _parameters.Select(_parameters => _parameters.Value);
            }
        }

        public int Count
        {
            get
            {
                return _parameters.Count;
            }
        }

        public bool ContainsKey(string key)
        {
            return _parameters.Any(p => p.Key == key);
        }

        public IEnumerator<KeyValuePair<string, IPluginParameter>> GetEnumerator()
        {
            return _parameters.AsReadOnly().GetEnumerator();
        }

        public bool TryGetValue(string key, out IPluginParameter value)
        {
            if (ContainsKey(key))
            {
                value = _parameters.First(p => p.Key == key).Value;
                return true;
            }
            else
            {
                value = null!;
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _parameters.AsReadOnly().GetEnumerator();
        }

        public PluginParameterCollection Clone()
        {
            var clone = new PluginParameterCollection();
            foreach (var parameter in _parameters)
            {
                if (parameter.Value is IPluginParameterInternal clonableParameter)
                {
                    clone.Add(clonableParameter.Clone());
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            return clone;
        }

        internal void Add(IPluginParameter parameter)
        {
            if (ContainsKey(parameter.Key))
            {
                throw new ArgumentException("A parameter with the same key already exists in the collection.");
            }

            _parameters.Add(new KeyValuePair<string, IPluginParameter>(parameter.Key, parameter));
        }
    }
}
