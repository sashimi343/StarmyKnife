using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("Grep")]
    public class GrepConverter : PluginBase, IConverter
    {
        private class ParameterKeys
        {
            public const string Pattern = "Pattern";
            public const string UseRegex = "UseRegex";
            public const string IgnoreCase = "IgnoreCase";
            public const string InvertMatch = "InvertMatch";
            public const string MatchedPartOnly = "MatchedPartOnly";
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var patternText = parameters[ParameterKeys.Pattern].GetValue<string>();
            var useRegex = parameters[ParameterKeys.UseRegex].GetValue<bool>();
            var ignoreCase = parameters[ParameterKeys.IgnoreCase].GetValue<bool>();
            var invertMatch = parameters[ParameterKeys.InvertMatch].GetValue<bool>();
            var matchedPartOnly = parameters[ParameterKeys.MatchedPartOnly].GetValue<bool>();

            var inputLines = input.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var pattern = CreateSearchPattern(patternText, useRegex, ignoreCase);
            List<string> outputLines = [];

            foreach (var line in inputLines)
            {
                var match = pattern.Match(line);
                var isOutput = invertMatch ? !match.Success : match.Success;

                if (isOutput)
                {
                    if (matchedPartOnly)
                    {
                        outputLines.Add(match.Value);
                    }
                    else
                    {
                        outputLines.Add(line);
                    }
                }
            }

            var output = string.Join(Environment.NewLine, outputLines);

            return PluginInvocationResult.OfSuccess(output);
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddTextParameter(ParameterKeys.Pattern);
            configuration.AddFlagParameter(ParameterKeys.UseRegex);
            configuration.AddFlagParameter(ParameterKeys.IgnoreCase);
            configuration.AddFlagParameter(ParameterKeys.InvertMatch);
            configuration.AddFlagParameter(ParameterKeys.MatchedPartOnly);
        }

        private Regex CreateSearchPattern(string patternText, bool useRegex, bool ignoreCase)
        {
            if (useRegex)
            {
                return new Regex(patternText, GetRegexOptions(ignoreCase));
            }
            else
            {
                return new Regex(Regex.Escape(patternText), GetRegexOptions(ignoreCase));
            }
        }

        private RegexOptions GetRegexOptions(bool ignoreCase)
        {
            var options = RegexOptions.Compiled;
            if (ignoreCase)
            {
                options |= RegexOptions.IgnoreCase;
            }

            return options;
        }
    }
}
