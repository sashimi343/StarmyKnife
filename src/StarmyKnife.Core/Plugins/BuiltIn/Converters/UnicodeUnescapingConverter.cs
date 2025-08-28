using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("Unicode unescape")]
    public class UnicodeUnescapingConverter : PluginBase, IConverter
    {
        public class ParameterKeys
        {
            public const string Prefix = "Prefix";
            public const string Delimiter = "Delimiter";
        }

        public enum PrefixType
        {
            Auto,
            [Display(Name = "\\u")]
            BackslashU,
            [Display(Name = "%u")]
            PercentU,
            [Display(Name = "U+")]
            UPlus,
        }

        public enum DelimiterType
        {
            None,
            Space,
            Comma,
            NewLine
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var prefixType = parameters[ParameterKeys.Prefix].GetValue<PrefixType>();
            var delimiterType = parameters[ParameterKeys.Delimiter].GetValue<DelimiterType>();

            var prefix = GetPrefix(prefixType, input);
            var delimiter = GetDelimiter(delimiterType);

            var sb = new StringBuilder();

            try
            {
                //
                var codePoints = GetCodePoints(input, delimiterType, prefix);

                foreach (var codePoint in codePoints)
                {
                    var codePointValue = System.Convert.ToInt32(codePoint, 16);
                    sb.Append((char)codePointValue);
                }

                return PluginInvocationResult.OfSuccess(sb.ToString());
            }
            catch (FormatException)
            {
                return PluginInvocationResult.OfFailure("Input contains invalid code points.");
            }
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
                case DelimiterType.NewLine:
                    return Environment.NewLine;
                default:
                    throw new ArgumentOutOfRangeException(nameof(delimiterType), delimiterType, null);
            }
        }

        private IEnumerable<string> GetCodePoints(string input, DelimiterType delimiterType, string prefix)
        {
            var delimiterText = GetDelimiter(delimiterType);

            switch (delimiterType)
            {
                case DelimiterType.None:
                    var itemLength = prefix.Length + 4;
                    return Enumerable.Range(0, input.Length / itemLength)
                                     .Select(i => input.Substring(i * itemLength, itemLength).Replace(prefix, ""));
                case DelimiterType.Space:
                case DelimiterType.Comma:
                case DelimiterType.NewLine:
                    return input.Split(new[] {delimiterText}, StringSplitOptions.RemoveEmptyEntries)
                                .Select(i => i.Replace(prefix, ""));
                default:
                    throw new ArgumentOutOfRangeException(nameof(delimiterType), delimiterType, null);
            }
        }
    }
}
