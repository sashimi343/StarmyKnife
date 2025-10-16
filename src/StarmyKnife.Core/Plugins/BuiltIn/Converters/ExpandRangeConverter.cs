using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("Expand Range")]
    public class ExpandRangeConverter : PluginBase, IConverter
    {
        public class ParameterKeys
        {
            public const string Delimiter = "Delimiter";
            public const string ItemType = "ItemType";
        }

        public enum DelimiterType
        {
            Comma,
            Space,
            Tab,
            NewLine,
            SemiColon,
            Colon
        }

        public enum ItemType
        {
            Letter,
            Hex
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var delimiterType = parameters[ParameterKeys.Delimiter].GetValue<DelimiterType>();
            var delimiter = GetDelimiter(delimiterType);
            var itemType = parameters[ParameterKeys.ItemType].GetValue<ItemType>();

            var items = SplitItems(input ?? string.Empty, delimiterType);
            var sb = new StringBuilder();
            foreach (var item in items)
            {
                var rangeParts = item.Split(['-'], StringSplitOptions.RemoveEmptyEntries);

                if (rangeParts.Length == 1)
                {
                    sb.Append(item);
                    sb.Append(delimiter);
                }
                else if (rangeParts.Length == 2)
                {
                    switch (itemType)
                    {
                        case ItemType.Letter:
                            if (rangeParts[0].Length != 1 || rangeParts[1].Length != 1)
                            {
                                return PluginInvocationResult.OfFailure($"Invalid letter range format '{item}'");
                            }
                            var startChar = rangeParts[0][0];
                            var endChar = rangeParts[1][0];

                            if (startChar > endChar)
                            {
                                (startChar, endChar) = (endChar, startChar);
                            }
                            for (char c = startChar; c <= endChar; c++)
                            {
                                sb.Append(c);
                            }
                            break;
                        case ItemType.Hex:
                            if (int.TryParse(rangeParts[0], NumberStyles.HexNumber, null, out int startValue) &&
                                int.TryParse(rangeParts[1], NumberStyles.HexNumber, null, out int endValue))
                            {
                                if (startValue > endValue)
                                {
                                    (startValue, endValue) = (endValue, startValue);
                                }

                                var minLength = Math.Min(rangeParts[0].Length, rangeParts[1].Length);
                                var formatString = $"X{minLength}";

                                for (int i = startValue; i <= endValue; i++)
                                {
                                    sb.Append(i.ToString(formatString));
                                    sb.Append(delimiter);
                                }
                            }
                            else
                            {
                                 return PluginInvocationResult.OfFailure($"Invalid hex range format '{item}'");
                            }
                            break;
                        default:
                            return PluginInvocationResult.OfFailure($"Unsupported item type '{itemType}'");
                    }

                    sb.Append(delimiter);
                }
                else
                {
                    return PluginInvocationResult.OfFailure($"Invalid range format '{item}'");
                }
            }

            if (sb.Length > 0 && delimiter.Length > 0)
            {
                // Remove the trailing delimiter
                sb.Length -= delimiter.Length;
            }

            return PluginInvocationResult.OfSuccess(sb.ToString());
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddListParameter<DelimiterType>(ParameterKeys.Delimiter, DelimiterType.Comma);
            configuration.AddListParameter<ItemType>(ParameterKeys.ItemType, ItemType.Letter);
        }

        private string[] SplitItems(string input, DelimiterType delimiterType)
        {
            return delimiterType switch
            {
                DelimiterType.Comma => input.Split([','], StringSplitOptions.RemoveEmptyEntries),
                DelimiterType.Space => input.Split([' '], StringSplitOptions.RemoveEmptyEntries),
                DelimiterType.Tab => input.Split(['\t'], StringSplitOptions.RemoveEmptyEntries),
                DelimiterType.NewLine => input.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries),
                DelimiterType.SemiColon => input.Split([';'], StringSplitOptions.RemoveEmptyEntries),
                DelimiterType.Colon => input.Split([':'], StringSplitOptions.RemoveEmptyEntries),
                _ => throw new ArgumentException("Invalid delimiter type")
            };
        }

        private string GetDelimiter(DelimiterType type)
        {
            return type switch
            {
                DelimiterType.Comma => ",",
                DelimiterType.Space => " ",
                DelimiterType.Tab => "\t",
                DelimiterType.NewLine => Environment.NewLine,
                DelimiterType.SemiColon => ";",
                DelimiterType.Colon => ":",
                _ => throw new ArgumentException("Invalid delimiter type")
            };
        }
    }
}
