using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("Unicode escape")]
    public class UnicodeEscapingConverter : PluginBase, IConverter
    {
        public class ParameterKeys
        {
            public const string Prefix = "Prefix";
            public const string Delimiter = "Delimiter";
        }

        public enum PrefixType
        {
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

            var sb = new StringBuilder();
            var prefix = GetPrefix(prefixType);
            var delimiter = GetDelimiter(delimiterType);

            foreach (var c in input)
            {
                sb.AppendFormat("{0}{1:X4}{2}", prefix, (int)c, delimiter);
            }

            RemoveTrailingDelimiter(sb, delimiterType);

            return PluginInvocationResult.OfSuccess(sb.ToString());
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddListParameter<PrefixType>(ParameterKeys.Prefix);
            configuration.AddListParameter<DelimiterType>(ParameterKeys.Delimiter);
        }

        private string GetPrefix(PrefixType prefixType)
        {
            switch (prefixType)
            {
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

        private void RemoveTrailingDelimiter(StringBuilder sb, DelimiterType delimiter)
        {
            var delimiterLength = GetDelimiter(delimiter).Length;
            if (delimiterLength > 0 && sb.Length >= delimiterLength)
            {
                sb.Remove(sb.Length - delimiterLength, delimiterLength);
            }
        }
    }
}
