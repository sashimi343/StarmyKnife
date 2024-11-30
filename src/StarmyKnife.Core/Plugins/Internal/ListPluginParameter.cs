using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarmyKnife.Core.Plugins.Internal
{
    public sealed class ListPluginParameter : IPluginParameter
    {
        private readonly string _key;
        private readonly string _name;
        private List<ListItem> _items;
        private int _selectedIndex;

        internal ListPluginParameter(string key, string name, IEnumerable<ListItem> items, int defaultIndex)
        {
            _key = key;
            _name = name;
            _items = items.ToList();
            _selectedIndex = defaultIndex;
        }

        public string Key => _key;

        public string Name => _name;

        public IReadOnlyList<ListItem> Items => _items.AsReadOnly();

        public IReadOnlyList<string> Labels => _items.Select(i => i.Label).ToList();

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (value < 0 || value >= _items.Count)
                {
                    throw new IndexOutOfRangeException(nameof(value));
                }

                _selectedIndex = value;
            }
        }

        public object SelectedValue => _items[_selectedIndex].Value;

        public T GetValue<T>()
        {
            if (SelectedValue is T)
            {
                return (T)SelectedValue;
            }
            else
            {
                throw new InvalidCastException("Cannot convert " + SelectedValue.GetType().Name + " to " + typeof(T).Name);

            }
        }

        public void SetValue(object value)
        {
            var index = _items.FindIndex(item => item.Value.Equals(value));
            if (index < 0)
            {
                throw new ArgumentException("Value not found in list");
            }
            _selectedIndex = index;
        }
    }
}
