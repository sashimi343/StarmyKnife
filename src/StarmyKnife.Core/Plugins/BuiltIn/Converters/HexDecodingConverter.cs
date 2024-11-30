using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("From Hex")]
    public class HexDecodingConverter : PluginBase, IConverter
    {
        private class ParameterKeys
        {
            public const string Encoding = "Encoding";
            public const string Delimiter = "Delimiter";
        }

        private enum DelimiterType
        {
            Auto,
            None,
            Space,
            Percent,
            Comma,
            SemiColon,
            Colon,
            LF,
            CRLF,
            [Display(Name = "0x")] ZeroX,
            [Display(Name = "0x with comma")] ZeroXWithComma,
            [Display(Name = "\\x")] X,
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var encoding = parameters[ParameterKeys.Encoding].GetValue<Encoding>();
            var delimiter = parameters[ParameterKeys.Delimiter].GetValue<DelimiterType>();

            var detectedDelimiter = delimiter == DelimiterType.Auto ? DetectDelimiter(input) : delimiter;

            var delimiterRemovedString = RemoveDelimiter(input, detectedDelimiter);
            var oneline = delimiterRemovedString.Replace("\\r", "").Replace("\\n", "");
            var inputBytes = new List<byte>();
            for (int i = 0; i < oneline.Length; i += 2)
            {
                var hex = oneline.Substring(i, 2);
                inputBytes.Add(System.Convert.ToByte(hex, 16));
            }

            var output = encoding.GetString(inputBytes.ToArray());

            return PluginInvocationResult.OfSuccess(output);
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddListParameter(ParameterKeys.Encoding, PluginEncoding.GetListItems(
                PluginEncoding.UTF8N,
                PluginEncoding.UTF16BE,
                PluginEncoding.UTF16LE,
                PluginEncoding.UTF32,
                PluginEncoding.ShiftJIS,
                PluginEncoding.EUCJP,
                PluginEncoding.ISO2022JP
            ));
            configuration.AddListParameter<DelimiterType>(ParameterKeys.Delimiter);
        }

        private DelimiterType DetectDelimiter(string input)
        {
            // Detect delimiters with prefix
            if (input.StartsWith("%"))
            {
                return DelimiterType.Percent;
            }
            if (input.StartsWith("0x"))
            {
                var suffixOfZeroX = input.Length > 5 ? input.Substring(4, 1) : "";
                return suffixOfZeroX switch
                {
                    "," => DelimiterType.ZeroXWithComma,
                    _ => DelimiterType.ZeroX,
                };
            }
            if (input.StartsWith("\\x"))
            {
                return DelimiterType.X;
            }

            // Detect CRLF
            if (input.Length > 4 && input.Substring(2, 2) == "\\r\\n")
            {
                return DelimiterType.CRLF;
            }

            // Detect delimiters with (single character ) suffix
            var suffix = input.Length > 3 ? input.Substring(2, 1) : "";
            return suffix switch
            {
                " " => DelimiterType.Space,
                "," => DelimiterType.Comma,
                ";" => DelimiterType.SemiColon,
                ":" => DelimiterType.Colon,
                "\n" => DelimiterType.LF,
                _ => DelimiterType.None,
            };
        }

        private string RemoveDelimiter(string input, DelimiterType delimiter)
        {
            switch (delimiter)
            {
                case DelimiterType.None:
                    return input;
                case DelimiterType.Space:
                    return input.Replace(" ", "");
                case DelimiterType.Percent:
                    return input.Replace("%", "");
                case DelimiterType.Comma:
                    return input.Replace(",", "");
                case DelimiterType.SemiColon:
                    return input.Replace(";", "");
                case DelimiterType.Colon:
                    return input.Replace(":", "");
                case DelimiterType.LF:
                    return input.Replace("\n", "");
                case DelimiterType.CRLF:
                    return input.Replace("\r\n", "");
                case DelimiterType.ZeroX:
                    return input.Replace("0x", "");
                case DelimiterType.ZeroXWithComma:
                    return input.Replace("0x", "").Replace(",", "");
                case DelimiterType.X:
                    return input.Replace("\\x", "");
                default:
                    throw new NotImplementedException($"Unsupported DelimiterType {delimiter}");
            }
        }
    }
}
