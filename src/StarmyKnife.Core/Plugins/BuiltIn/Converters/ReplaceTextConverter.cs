using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("Replace text (simple substitution)")]
    public class ReplaceTextConverter : PluginBase, IConverter
    {
        private class ParameterKeys
        {
            public const string Pattern = "Pattern";
            public const string Replacement = "Replacement";
            public const string UseRegex = "UseRegex";
            public const string IgnoreCase = "IgnoreCase";
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var patternText = parameters[ParameterKeys.Pattern].GetValue<string>();
            var replacementText = parameters[ParameterKeys.Replacement].GetValue<string>();
            var useRegex = parameters[ParameterKeys.UseRegex].GetValue<bool>();

            var options = GetRegexOptions(parameters);
            Regex pattern;
            Regex replacement;

            if (useRegex)
            {
                pattern = new Regex(patternText, options);
                replacement = new Regex(replacementText, options);
            }
            else
            {
                pattern = new Regex(Regex.Escape(patternText), options);
                replacement = new Regex(Regex.Escape(replacementText), options);
            }

            var result = pattern.Replace(input, replacementText);

            return PluginInvocationResult.OfSuccess(result);
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddTextParameter(ParameterKeys.Pattern);
            configuration.AddTextParameter(ParameterKeys.Replacement);
            configuration.AddFlagParameter(ParameterKeys.UseRegex);
            configuration.AddFlagParameter(ParameterKeys.IgnoreCase);
        }

        private RegexOptions GetRegexOptions(PluginParameterCollection parameters)
        {
            var options = RegexOptions.None;
            if (parameters[ParameterKeys.IgnoreCase].GetValue<bool>())
            {
                options |= RegexOptions.IgnoreCase;
            }

            return options;
        }
    }
}
