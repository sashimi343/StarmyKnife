using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using StarmyKnife.Contracts.Services;
using StarmyKnife.Events;

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

        public bool ClickOutputToCopy
        {
            get { return GetSettings<bool>(nameof(ClickOutputToCopy)); }
            set { SetSettings<bool>(nameof(ClickOutputToCopy), value); }
        }

        public bool UsePrettyValidatorAsConverter
        {
            get { return GetSettings<bool>(nameof(UsePrettyValidatorAsConverter)); }
            set { SetSettings<bool>(nameof(UsePrettyValidatorAsConverter), value); }
        }

        public string XPathSearchHistoriesText
        {
            get { return GetSettings<string>(nameof(XPathSearchHistoriesText)); }
            set { SetSettings<string>(nameof(XPathSearchHistoriesText), value); }
        }

        public string JsonPathSearchHistoriesText
        {
            get { return GetSettings<string>(nameof(JsonPathSearchHistoriesText)); }
            set { SetSettings<string>(nameof(JsonPathSearchHistoriesText), value); }
        }

        public FontFamily IOFontFamily
        {
            get
            {
                var fontFamilyName = GetSettings<string>(nameof(IOFontFamily));
                var fontFamily = Fonts.SystemFontFamilies.FirstOrDefault(f => f.Source == fontFamilyName, new FontFamily("Segoe UI"));
                return fontFamily;
            }
            set
            {
                SetSettings<string>(nameof(IOFontFamily), value.Source);
            }
        }

        public int IOFontSize
        {
            get { return GetSettings<int>(nameof(IOFontSize), 12); }
            set { SetSettings<int>(nameof(IOFontSize), value); }
        }

        private T GetSettings<T>(string key, T defaultValue = default)
        {
            try
            {
                if (!_appProperties.ContainsKey(key))
                {
                    return defaultValue;
                }

                object value = Type.GetTypeCode(typeof(T)) switch
                {
                    TypeCode.Int32 => Convert.ToInt32(_appProperties[key]),
                    _ => _appProperties[key],
                };

                return (T)value;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        private void SetSettings<T>(string key, T value)
        {
            _appProperties[key] = value;
        }
    }
}
