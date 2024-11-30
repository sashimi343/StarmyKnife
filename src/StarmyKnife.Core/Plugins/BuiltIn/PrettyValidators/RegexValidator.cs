using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Plugins.BuiltIn.PrettyValidators
{
    [StarmyKnifePlugin("Regular expression (.NET)")]
    public class RegexValidator : PluginBase, IPrettyValidator
    {
        public bool CanPrettify => false;

        public bool CanMinify => false;

        public PluginInvocationResult Minify(string input, PluginParameterCollection parameters)
        {
            throw new NotImplementedException();
        }

        public PluginInvocationResult Prettify(string input, PluginParameterCollection parameters)
        {
            throw new NotImplementedException();
        }

        public ValidationResult Validate(string input, PluginParameterCollection parameters)
        {
            try
            {
                var regex = new Regex(input);
                return ValidationResult.OfSuccess();
            }
            catch (Exception e)
            {
                return ValidationResult.OfFailure(e.Message);
            }
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            // No parameters
        }
    }
}
