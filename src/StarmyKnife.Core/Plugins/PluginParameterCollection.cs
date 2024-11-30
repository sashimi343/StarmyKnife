using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarmyKnife.Core.Plugins.Internal;

namespace StarmyKnife.Core.Plugins
{
    public class PluginParameterCollection : IReadOnlyDictionary<string, IPluginParameter>
    {
        private List<KeyValuePair<string, IPluginParameter>> _parameters;

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
                value = null;
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
                if (parameter.Value is TextPluginParameter textParameter)
                {
                    clone.Add(new TextPluginParameter(textParameter.Key, textParameter.Name, textParameter.Text));
                }
                else if (parameter.Value is FlagPluginParameter flagParameter)
                {
                    clone.Add(new FlagPluginParameter(flagParameter.Key, flagParameter.Name, flagParameter.Value));
                }
                else if (parameter.Value is ListPluginParameter listParameter)
                {
                    clone.Add(new ListPluginParameter(listParameter.Key, listParameter.Name, listParameter.Items, listParameter.SelectedIndex));
                }
                else if (parameter.Value is NumericPluginParameter numberParameter)
                {
                    clone.Add(new NumericPluginParameter(numberParameter.Key, numberParameter.Name, numberParameter.IsInteger, numberParameter.Value));
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
