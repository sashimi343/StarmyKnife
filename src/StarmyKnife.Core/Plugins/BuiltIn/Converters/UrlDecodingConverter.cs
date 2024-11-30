using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("URL Decode")]
    public class UrlDecodingConverter : PluginBase, IConverter
    {
        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var decodedString = Uri.UnescapeDataString(input);

            return PluginInvocationResult.OfSuccess(decodedString);
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            // No parameters
        }
    }
}
