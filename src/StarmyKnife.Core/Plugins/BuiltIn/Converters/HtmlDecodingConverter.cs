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
            var output = HtmlEncodingHelper.FromHtmlEntities(input);

            return PluginInvocationResult.OfSuccess(output);
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            // No parameters
        }
    }
}
