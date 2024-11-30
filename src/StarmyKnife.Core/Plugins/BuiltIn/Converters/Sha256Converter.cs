using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("SHA-256")]
    public class Sha256Converter : PluginBase, IConverter
    {
        private class ParameterKeys
        {
            public const string Encoding = "Encoding";
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var encoding = parameters[ParameterKeys.Encoding].GetValue<Encoding>();
            byte[] inputBytes = encoding.GetBytes(input);

            using var hashAlgorithm = SHA256.Create();

            byte[] hashBytes = hashAlgorithm.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return PluginInvocationResult.OfSuccess(sb.ToString());
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
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
    }
}
