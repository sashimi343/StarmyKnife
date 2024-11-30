using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("From Punycode")]
    public class PunycodeDecodingConverter : PluginBase, IConverter
    {
        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var idn = new IdnMapping();
            var result = idn.GetUnicode(input);

            return PluginInvocationResult.OfSuccess(result);
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            // No parameters
        }
    }
}
