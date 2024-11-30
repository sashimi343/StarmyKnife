using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins
{
    public class ListItem
    {
        public string Label { get; }
        public object Value { get; }

        public ListItem(string label, object value)
        {
            Label = label;
            Value = value;
        }
    }
}
