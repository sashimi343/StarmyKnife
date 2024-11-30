using StarmyKnife.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("HTML Decode")]
    public class HtmlDecodingConverter : PluginBase, IConverter
    {
        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var decodeHex = HtmlEncodingHelper.FromHexEntities(input);
            var decodeNumeric = HtmlEncodingHelper.FromNumericEntities(decodeHex);
            var decodeNamed = HtmlEncodingHelper.FromNamedEntities(decodeNumeric);

            return PluginInvocationResult.OfSuccess(decodeNamed);
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            // No parameters
        }
    }
}
