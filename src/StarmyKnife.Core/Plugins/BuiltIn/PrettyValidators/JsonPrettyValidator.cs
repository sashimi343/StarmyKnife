using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace StarmyKnife.Core.Plugins.BuiltIn.PrettyValidators
{
    [StarmyKnifePlugin("JSON")]
    public class JsonPrettyValidator : PluginBase, IPrettyValidator
    {
        public bool CanPrettify => true;
        public bool CanMinify => true;

        public PluginInvocationResult Minify(string input, PluginParameterCollection parameters)
        {
            try
            {
                var parsedJson = JsonConvert.DeserializeObject(input);
                var prettyJson = JsonConvert.SerializeObject(parsedJson, Formatting.None);
                return PluginInvocationResult.OfSuccess(prettyJson);

            }
            catch (Exception ex)
            {
                return PluginInvocationResult.OfFailure(ex.Message);
            }
        }

        public PluginInvocationResult Prettify(string input, PluginParameterCollection parameters)
        {
            try
            {
                var parsedJson = JsonConvert.DeserializeObject(input);
                var prettyJson = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
                return PluginInvocationResult.OfSuccess(prettyJson);

            }
            catch (Exception ex)
            {
                return PluginInvocationResult.OfFailure(ex.Message);
            }
        }

        public ValidationResult Validate(string input, PluginParameterCollection parameters)
        {
            try
            {
                var parsedJson = JsonConvert.DeserializeObject(input);
                return ValidationResult.OfSuccess();
            }
            catch (Exception ex)
            {
                return ValidationResult.OfFailure(ex.Message);
            }
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
        }
    }
}
