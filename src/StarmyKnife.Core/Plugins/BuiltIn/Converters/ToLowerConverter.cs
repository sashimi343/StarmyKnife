using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("To Lower Case")]
    public class ToLowerConverter : PluginBase, IConverter
    {
        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            return PluginInvocationResult.OfSuccess(input.ToLower());
        }
        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            // No parameters needed
        }
    }
}
