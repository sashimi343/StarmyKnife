using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("To Upper Case")]
    public class ToUpperConverter : PluginBase, IConverter
    {
        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            return PluginInvocationResult.OfSuccess(input.ToUpper());
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            // No parameters needed
        }
    }
}
