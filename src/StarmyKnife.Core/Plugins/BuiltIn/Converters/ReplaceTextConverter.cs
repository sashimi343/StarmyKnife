using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("Replace text (simple substitution)")]
    public class ReplaceTextConverter : PluginBase, IConverter
    {
        public class ParameterKeys
        {
            public const string Pattern = "Pattern";
            public const string Replacement = "Replacement";
            public const string UseRegex = "UseRegex";
            public const string IgnoreCase = "IgnoreCase";
            public const string RegexMode = "RegexMode";
        }

        public enum RegexMode
        {
            SingleLine,
            MultiLine
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var patternText = parameters[ParameterKeys.Pattern].GetValue<string>();
            var replacementText = parameters[ParameterKeys.Replacement].GetValue<string>();
            var useRegex = parameters[ParameterKeys.UseRegex].GetValue<bool>();

            if (string.IsNullOrEmpty(patternText))
            {
                return PluginInvocationResult.OfSuccess(input);
            }

            var options = GetRegexOptions(useRegex, parameters);
            Regex pattern;
            string replacement;

            try
            {
                if (useRegex)
                {
                    pattern = new Regex(patternText, options);
                    replacement = UnescapeSequence(replacementText);
                }
                else
                {
                    pattern = new Regex(Regex.Escape(patternText), options);
                    replacement = replacementText;
                }
            }
            catch (RegexParseException ex)
            {
                return PluginInvocationResult.OfFailure(ex.Message);
            }

            var result = pattern.Replace(input, replacement);

            return PluginInvocationResult.OfSuccess(result);
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddTextParameter(ParameterKeys.Pattern);
            configuration.AddTextParameter(ParameterKeys.Replacement);
            configuration.AddFlagParameter(ParameterKeys.UseRegex);
            configuration.AddListParameter(ParameterKeys.RegexMode, RegexMode.SingleLine);
            configuration.AddFlagParameter(ParameterKeys.IgnoreCase);
        }

        private RegexOptions GetRegexOptions(bool useRegex, PluginParameterCollection parameters)
        {
            var options = RegexOptions.None;
            if (parameters[ParameterKeys.IgnoreCase].GetValue<bool>())
            {
                options |= RegexOptions.IgnoreCase;
            }

            // If not using regex, no need to set other options
            if (!useRegex)
            {
                return options;
            }

            var regexMode = parameters[ParameterKeys.RegexMode].GetValue<RegexMode>();
            switch (regexMode)
            {
                case RegexMode.SingleLine:
                    options |= RegexOptions.Singleline;
                    break;
                case RegexMode.MultiLine:
                    options |= RegexOptions.Multiline;
                    break;
            }

            return options;
        }

        private string UnescapeSequence(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return input.Replace("\\n", "\n")
                        .Replace("\\r", "\r")
                        .Replace("\\t", "\t");
        }
    }
}
