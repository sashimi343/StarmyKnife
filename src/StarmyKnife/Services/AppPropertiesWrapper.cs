using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StarmyKnife.Contracts.Services;

namespace StarmyKnife.Services
{
    public class AppPropertiesWrapper : IAppPropertiesWrapper
    {
        public object this[string key]
        {
            get
            {
                return App.Current.Properties.Contains(key) ? App.Current.Properties[key] : null;
            }
            set
            {
                App.Current.Properties[key] = value;
            }

        }

        public bool ContainsKey(string key)
        {
            return App.Current.Properties.Contains(key);
        }
    }
}
