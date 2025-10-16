using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("Unique")]
    public class UniqueConverter : PluginBase, IConverter
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

        public class ParameterKeys
        {
            public const string Delimiter = "Delimiter";
            public const string Collation = "Collation";
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var delimiterType = parameters[ParameterKeys.Delimiter].GetValue<DelimiterType>();
            var collation = parameters[ParameterKeys.Collation].GetValue<StringComparison>();

            var inputItems = SplitInput(input ?? string.Empty, delimiterType);
            var uniqueItems = inputItems.Distinct(StringComparer.FromComparison(collation));
            var output = string.Join(GetDelimiter(delimiterType), uniqueItems);

            return PluginInvocationResult.OfSuccess(output);
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddListParameter<DelimiterType>(ParameterKeys.Delimiter, DelimiterType.NewLine);
            configuration.AddListParameter<StringComparison>(ParameterKeys.Collation, StringComparison.Ordinal);
        }

        public string[] SplitInput(string input, DelimiterType delimiterType)
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

        public string GetDelimiter(DelimiterType type)
        {
            return type switch
            {
                DelimiterType.Comma => ",",
                DelimiterType.Space => " ",
                DelimiterType.Tab => "\t",
                DelimiterType.NewLine => "\r\n",
                DelimiterType.SemiColon => ";",
                DelimiterType.Colon => ":",
                DelimiterType.Nothing => "",
                _ => throw new ArgumentException("Invalid delimiter type")
            };
        }
    }
}
