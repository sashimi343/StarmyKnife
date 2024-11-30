using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("To Hex")]
    public class HexEncodingConverter : PluginBase, IConverter
    {
        private class ParameterKeys
        {
            public const string Encoding = "Encoding";
            public const string Delimiter = "Delimiter";
            public const string BytesPerLine = "BytesPerLine";
        }

        private enum DelimiterType
        {
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
            var bytesPerLine = parameters[ParameterKeys.BytesPerLine].GetValue<int>();

            var inputBytes = encoding.GetBytes(input);
            var sb = new StringBuilder();
            int numBytesInLine = 0;

            foreach (byte currentByte in inputBytes)
            {
                AppendWithDelimiter(sb, currentByte, delimiter);

                if (bytesPerLine > 0)
                {
                    numBytesInLine++;
                    if (numBytesInLine >= bytesPerLine)
                    {
                        sb.AppendLine();
                        numBytesInLine = 0;
                    }
                }
            }

            RemoveTrailingDelimiter(sb, delimiter);

            return PluginInvocationResult.OfSuccess(sb.ToString());
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
            configuration.AddIntegerParameter(ParameterKeys.BytesPerLine, 0);
        }

        private void AppendWithDelimiter(StringBuilder sb, byte newByte, DelimiterType delimiter)
        {
            switch (delimiter)
            {
                case DelimiterType.None:
                    sb.Append(newByte.ToString("X2"));
                    break;
                case DelimiterType.Space:
                    sb.AppendFormat("{0} ", newByte.ToString("X2"));
                    break;
                case DelimiterType.Percent:
                    sb.AppendFormat("%{0}", newByte.ToString("X2"));
                    break;
                case DelimiterType.Comma:
                    sb.AppendFormat("{0},", newByte.ToString("X2"));
                    break;
                case DelimiterType.SemiColon:
                    sb.AppendFormat("{0};", newByte.ToString("X2"));
                    break;
                case DelimiterType.Colon:
                    sb.AppendFormat("{0}:", newByte.ToString("X2"));
                    break;
                case DelimiterType.LF:
                    sb.AppendFormat("{0}\n", newByte.ToString("X2"));
                    break;
                case DelimiterType.CRLF:
                    sb.AppendFormat("{0}\r\n", newByte.ToString("X2"));
                    break;
                case DelimiterType.ZeroX:
                    sb.AppendFormat("0x{0}", newByte.ToString("X2"));
                    break;
                case DelimiterType.ZeroXWithComma:
                    sb.AppendFormat("0x{0},", newByte.ToString("X2"));
                    break;
                case DelimiterType.X:
                    sb.AppendFormat("\\x{0}", newByte.ToString("X2"));
                    break;
                default:
                    throw new NotImplementedException($"Unsupported DelimiterType {delimiter}");
            }
        }

        private void RemoveTrailingDelimiter(StringBuilder sb, DelimiterType delimiter)
        {
            switch (delimiter)
            {
                case DelimiterType.Space:
                case DelimiterType.Percent:
                case DelimiterType.Comma:
                case DelimiterType.SemiColon:
                case DelimiterType.Colon:
                case DelimiterType.LF:
                    // Single character suffix
                    sb.Remove(sb.Length - 1, 1);
                    break;
                case DelimiterType.CRLF:
                    // Double character suffix
                    sb.Remove(sb.Length - 2, 2);
                    break;
                default:
                    // no suffix
                    break;
            }
        }
    }
}
