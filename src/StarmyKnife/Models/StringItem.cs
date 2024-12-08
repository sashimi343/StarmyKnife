using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Models
{
    public class StringItem
    {
        public string Value { get; set; }

        public static implicit operator String(StringItem item)
        {
            return item.Value;
        }

        public static implicit operator StringItem(string value)
        {
            return new StringItem { Value = value };
        }
    }

}
