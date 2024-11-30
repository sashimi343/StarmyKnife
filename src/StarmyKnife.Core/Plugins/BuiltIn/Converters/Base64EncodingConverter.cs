using StarmyKnife.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("To Base64")]
    public class Base64EncodingConverter : PluginBase, IConverter
    {
        private class ParameterKeys
        {
            public const string CharacterSet = "CharacterSet";
            public const string Encoding = "Encoding";
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var characterSet = parameters[ParameterKeys.CharacterSet].GetValue<Base64CharacterSet>();
            var encoding = parameters[ParameterKeys.Encoding].GetValue<Encoding>();

            var bytes = encoding.GetBytes(input);
            var base64String = System.Convert.ToBase64String(bytes);
            string output = characterSet switch
            {
                Base64CharacterSet.UrlSafe => ToUrlSafeBase64(base64String),
                Base64CharacterSet.FilenameSafe => ToFilenameSafeBase64(base64String),
                _ => base64String
            };

            return PluginInvocationResult.OfSuccess(output);
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
        }

        private string ToUrlSafeBase64(string base64String)
        {
            return base64String.Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        private string ToFilenameSafeBase64(string base64String)
        {
            return base64String.Replace('/', '-');
        }
    }
}
