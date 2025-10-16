using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("Sort")]
    public class SortConverter : PluginBase, IConverter
    {
        public class ParameterKeys
        {
            public const string Delimiter = "Delimiter";
            public const string Collation = "Collation";
            public const string Descending = "Descending";
        }

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

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            input ??= string.Empty;
            var delimiterType = parameters[ParameterKeys.Delimiter].GetValue<DelimiterType>();
            var collation = parameters[ParameterKeys.Collation].GetValue<StringComparison>();
            var descending = parameters[ParameterKeys.Descending].GetValue<bool>();

            var inputItems = SplitInput(input, delimiterType);
            var sortedItems = descending
                ? inputItems.OrderByDescending(item => item, StringComparer.FromComparison(collation))
                : inputItems.OrderBy(item => item, StringComparer.FromComparison(collation));

            var output = string.Join(GetDelimiter(delimiterType), sortedItems);

            return PluginInvocationResult.OfSuccess(output);
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddListParameter<DelimiterType>(ParameterKeys.Delimiter, DelimiterType.NewLine);
            configuration.AddListParameter<StringComparison>(ParameterKeys.Collation, StringComparison.Ordinal);
            configuration.AddFlagParameter(ParameterKeys.Descending, false);
        }

        private string[] SplitInput(string input, DelimiterType delimiterType)
        {
            return delimiterType switch
            {
                DelimiterType.Comma => input.Split([','], StringSplitOptions.RemoveEmptyEntries),
                DelimiterType.Space => input.Split([' '], StringSplitOptions.RemoveEmptyEntries),
                DelimiterType.Tab => input.Split(['\t'], StringSplitOptions.RemoveEmptyEntries),
                DelimiterType.NewLine => input.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries),
                DelimiterType.SemiColon => input.Split([';'], StringSplitOptions.RemoveEmptyEntries),
                DelimiterType.Colon => input.Split([':'], StringSplitOptions.RemoveEmptyEntries),
                DelimiterType.Nothing => input.ToCharArray().Select(c => c.ToString()).ToArray(),
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
                DelimiterType.Nothing => "",
                _ => throw new ArgumentException("Invalid delimiter type")
            };
        }
    }
}
