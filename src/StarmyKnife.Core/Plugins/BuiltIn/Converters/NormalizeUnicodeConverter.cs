using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("Normalize Unicode")]
    public class NormalizeUnicodeConverter : PluginBase, IConverter
    {
        public class ParameterKeys
        {
            public const string Form = "Form";
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var form = parameters[ParameterKeys.Form].GetValue<NormalizationForm>();
            var output = input.Normalize(form);
            return PluginInvocationResult.OfSuccess(output);
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddListParameter<NormalizationForm>(ParameterKeys.Form, NormalizationForm.FormD);
        }
    }
}
