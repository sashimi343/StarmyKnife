using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StarmyKnife.Contracts.Services;

namespace StarmyKnife.Models
{
    public class UserSettings
    {
        private readonly IAppPropertiesWrapper _appProperties;

        public UserSettings(IAppPropertiesWrapper appProperties)
        {
            _appProperties = appProperties;
        }

        public bool EnableAutoConvertByDefault
        {
            get { return GetSettings<bool>(nameof(EnableAutoConvertByDefault)); }
            set { SetSettings<bool>(nameof(EnableAutoConvertByDefault), value); }
        }

        private T GetSettings<T>(string key, T defaultValue = default)
        {
            return _appProperties.ContainsKey(key) ? (T)App.Current.Properties[key] : defaultValue;
        }

        private void SetSettings<T>(string key, T value)
        {
            _appProperties[key] = value;
        }
    }
}
