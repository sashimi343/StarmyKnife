using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("Split & Join")]
    public class SplitAndJoinConverter : PluginBase, IConverter
    {
        public enum DelimiterType
        {
            Comma,
            Space,
            Tab,
            NewLine,
            SemiColon,
            Colon,
            Nothing
        }

        public enum EnclosureType
        {
            None,
            DoubleQuotes,
            SingleQuotes,
            Backticks,
            RemoveQuotes,
        }

        public class ParameterKeys
        {
            public const string SplitDelimiter = "SplitDelimiter";
            public const string JoinDelimiter = "JoinDelimiter";
            public const string Enclosure = "Enclosure";
            public const string RemoveEmptyEntries = "RemoveEmptyEntries";
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var splitDelimiterType = parameters[ParameterKeys.SplitDelimiter].GetValue<DelimiterType>();
            var joinDelimiterType = parameters[ParameterKeys.JoinDelimiter].GetValue<DelimiterType>();
            var enclosureType = parameters[ParameterKeys.Enclosure].GetValue<EnclosureType>();
            var removeEmptyEntries = parameters[ParameterKeys.RemoveEmptyEntries].GetValue<bool>();

            var splitDelimiter = GetDelimiter(splitDelimiterType);
            var joinDelimiter = GetDelimiter(joinDelimiterType);
            var splitOptions = removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;

            var inputItems = string.IsNullOrEmpty(splitDelimiter) ? SplitChars(input) : input.Split([splitDelimiter], splitOptions);
            var enclosedItems = ApplyEnclosure(inputItems, enclosureType);
            var output = string.Join(joinDelimiter, enclosedItems);

            return PluginInvocationResult.OfSuccess(output);
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddListParameter<DelimiterType>(ParameterKeys.SplitDelimiter, DelimiterType.Comma);
            configuration.AddListParameter<DelimiterType>(ParameterKeys.JoinDelimiter, DelimiterType.NewLine);
            configuration.AddListParameter<EnclosureType>(ParameterKeys.Enclosure, EnclosureType.None);
            configuration.AddFlagParameter(ParameterKeys.RemoveEmptyEntries, true);
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
                DelimiterType.Nothing => "",
                _ => throw new ArgumentException("Invalid delimiter type")
            };
        }

        private string[] SplitChars(string input)
        {
            var stringInfo = new StringInfo(input);
            var inputItems = new string[stringInfo.LengthInTextElements];
            for (int i = 0; i < stringInfo.LengthInTextElements; i++)
            {
                inputItems[i] = stringInfo.SubstringByTextElements(i, 1);
            }

            return inputItems;
        }

        private string[] ApplyEnclosure(string[] items, EnclosureType enclosureType)
        {
            return enclosureType switch
            {
                EnclosureType.None => items,
                EnclosureType.DoubleQuotes => items.Select(i => $"\"{i}\"").ToArray(),
                EnclosureType.SingleQuotes => items.Select(i => $"'{i}'").ToArray(),
                EnclosureType.Backticks => items.Select(i => $"`{i}`").ToArray(),
                EnclosureType.RemoveQuotes => items.Select(i => i.Trim('\"', '\'', '`')).ToArray(),
                _ => throw new ArgumentException("Invalid enclosure type")
            };
        }
    }
}
