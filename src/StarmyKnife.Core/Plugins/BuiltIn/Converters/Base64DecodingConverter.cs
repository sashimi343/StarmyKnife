using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("From Base64")]
    public class Base64DecodingConverter : PluginBase, IConverter
    {
        private class ParameterKeys
        {
            public const string CharacterSet = "CharacterSet";
            public const string Encoding = "Encoding";
            public const string StrictMode = "StrictMode";
            public const string IgnoreInvalidCharacters = "IgnoreInvalidCharacters";
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var characterSet = parameters[ParameterKeys.CharacterSet].GetValue<Base64CharacterSet>();
            var encoding = parameters[ParameterKeys.Encoding].GetValue<Encoding>();
            var ignoreInvalidCharacters = parameters[ParameterKeys.IgnoreInvalidCharacters].GetValue<bool>();
            var strictMode = parameters[ParameterKeys.StrictMode].GetValue<bool>();

            var decodedBytes = System.Convert.FromBase64String(input);
            var decodedString = encoding.GetString(decodedBytes);

            return PluginInvocationResult.OfSuccess(decodedString);
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddListParameter(ParameterKeys.CharacterSet, Base64CharacterSet.Standard);
            configuration.AddListParameter(
                ParameterKeys.Encoding,
                PluginEncoding.GetListItems(PluginEncoding.UTF8,
                                            PluginEncoding.UTF16BE,
                                            PluginEncoding.UTF16LE,
                                            PluginEncoding.ShiftJIS,
                                            PluginEncoding.EUCJP,
                                            PluginEncoding.ISO2022JP)
            );
            configuration.AddFlagParameter(ParameterKeys.IgnoreInvalidCharacters, true);
            configuration.AddFlagParameter(ParameterKeys.StrictMode, false);
        }
    }
}
