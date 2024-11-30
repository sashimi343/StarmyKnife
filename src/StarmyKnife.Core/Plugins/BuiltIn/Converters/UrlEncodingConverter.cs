using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("URL Encode")]
    public class UrlEncodingConverter : PluginBase, IConverter
    {
        private class ParameterKeys
        {
            public const string EscapeWhitespace = "EscapeWhiteSpace";
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var escapeWhitespace = parameters[ParameterKeys.EscapeWhitespace].GetValue<bool>();

            var encodingString = escapeWhitespace ? Uri.EscapeDataString(input) : WebUtility.UrlEncode(input);

            return PluginInvocationResult.OfSuccess(encodingString);
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddFlagParameter(ParameterKeys.EscapeWhitespace, false);
        }
    }
}
