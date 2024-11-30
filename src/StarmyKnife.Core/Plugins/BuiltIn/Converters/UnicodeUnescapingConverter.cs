using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("Unicode unescape")]
    public class UnicodeUnescapingConverter : PluginBase, IConverter
    {
        private class ParameterKeys
        {
            public const string Prefix = "Prefix";
            public const string Delimiter = "Delimiter";
        }

        private enum PrefixType
        {
            Auto,
            [Display(Name = "\\u")]
            BackslashU,
            [Display(Name = "%u")]
            PercentU,
            [Display(Name = "U+")]
            UPlus,
        }

        private enum DelimiterType
        {
            None,
            Space,
            Comma,
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var prefixType = parameters[ParameterKeys.Prefix].GetValue<PrefixType>();
            var delimiterType = parameters[ParameterKeys.Delimiter].GetValue<DelimiterType>();

            var prefix = GetPrefix(prefixType, input);
            var delimiter = GetDelimiter(delimiterType);

            var sb = new StringBuilder();
            var codePoints = input.Replace("\\r", "").Replace("\\n", "").Split(new[] {delimiter}, StringSplitOptions.None);

            foreach (var codePoint in codePoints)
            {
                if (codePoint.StartsWith(prefix))
                {
                    var code = codePoint.Substring(prefix.Length);
                    if (code.Length == 4)
                    {
                        sb.Append((char)int.Parse(code));
                    }
                    else
                    {
                        return PluginInvocationResult.OfFailure($"Invalid code point: {codePoint}");
                    }
                }
                else
                {
                    return PluginInvocationResult.OfFailure($"Invalid code point: {codePoint}");
                }
            }

            return PluginInvocationResult.OfSuccess(sb.ToString());
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddListParameter<PrefixType>(ParameterKeys.Prefix);
            configuration.AddListParameter<DelimiterType>(ParameterKeys.Delimiter);
        }

        private string GetPrefix(PrefixType prefixType, string input)
        {
            switch (prefixType)
            {
                case PrefixType.Auto:
                    // Detect the prefix
                    if (input.StartsWith("\\u"))
                    {
                        return "\\u";
                    }
                    if (input.StartsWith("%u"))
                    {
                        return "%u";
                    }
                    if (input.StartsWith("U+"))
                    {
                        return "U+";
                    }

                    // Default to \u
                    return "\\u";
                case PrefixType.BackslashU:
                    return "\\u";
                case PrefixType.PercentU:
                    return "%u";
                case PrefixType.UPlus:
                    return "U+";
                default:
                    throw new ArgumentOutOfRangeException(nameof(prefixType), prefixType, null);
            }
        }

        private string GetDelimiter(DelimiterType delimiterType)
        {
            switch (delimiterType)
            {
                case DelimiterType.None:
                    return "";
                case DelimiterType.Space:
                    return " ";
                case DelimiterType.Comma:
                    return ",";
                default:
                    throw new ArgumentOutOfRangeException(nameof(delimiterType), delimiterType, null);
            }
        }

    }
}
