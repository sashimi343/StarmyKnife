using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("From Base64")]
    public class Base64DecodingConverter : PluginBase, IConverter
    {
        public class ParameterKeys
        {
            public const string CharacterSet = "CharacterSet";
            public const string Encoding = "Encoding";
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var characterSet = parameters[ParameterKeys.CharacterSet].GetValue<Base64CharacterSet>();
            var encoding = parameters[ParameterKeys.Encoding].GetValue<Encoding>();

            try
            {
                var processedInput = characterSet switch
                {
                    Base64CharacterSet.UrlSafe => FromUrlSafeBase64(input),
                    Base64CharacterSet.FilenameSafe => FromFilenameSafeBase64(input),
                    _ => input
                };

                var decodedBytes = System.Convert.FromBase64String(processedInput);
                var decodedString = encoding.GetString(decodedBytes);

                return PluginInvocationResult.OfSuccess(decodedString);
            }
            catch (Exception e) when (e is FormatException || e is ArgumentException || e is DecoderFallbackException)
            {
                return PluginInvocationResult.OfFailure(e.Message);
            }
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

        private string FromUrlSafeBase64(string base64String)
        {
            return base64String.Replace('-', '+').Replace('_', '/').PadRight(base64String.Length + (4 - base64String.Length % 4) % 4, '=');
        }

        private string FromFilenameSafeBase64(string base64String)
        {
            return base64String.Replace('-', '/');
        }
    }
}
