using System;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("To Quoted Printable (QP encoding)")]
    public class ToQuotedPrintableConverter : PluginBase, IConverter
    {
        public class ParameterKeys
        {
            public const string Encoding = "Encoding";
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            const int maxLineLength = 76;

            var encoding = parameters[ParameterKeys.Encoding].GetValue<Encoding>();

            // treat null as empty string
            input ??= string.Empty;

            byte[] bytes;
            try
            {
                bytes = encoding.GetBytes(input);
            }
            catch (EncoderFallbackException ex)
            {
                return PluginInvocationResult.OfFailure(ex.Message);
            }

            var sb = new StringBuilder();
            int currentLineLength = 0;

            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];

                // normalize line endings to CRLF
                if (b == 0x0D)
                {
                    if (i + 1 < bytes.Length && bytes[i + 1] == 0x0A)
                    {
                        sb.Append("\r\n");
                        i++;
                    }
                    else
                    {
                        sb.Append("\r\n");
                    }
                    currentLineLength = 0;
                    continue;
                }
                if (b == 0x0A)
                {
                    sb.Append("\r\n");
                    currentLineLength = 0;
                    continue;
                }

                // trailing space or tab must be encoded
                bool spaceOrTabAtLineEnd = false;
                if (b == 0x20 || b == 0x09)
                {
                    if (i + 1 == bytes.Length)
                    {
                        spaceOrTabAtLineEnd = true;
                    }
                    else
                    {
                        byte next = bytes[i + 1];
                        if (next == 0x0D || next == 0x0A)
                            spaceOrTabAtLineEnd = true;
                    }
                }

                string piece;
                if ((33 <= b && b <= 60) || (62 <= b && b <= 126))
                {
                    // (Literal representation) Octets with decimal values of 33 through 60 inclusive, and 62 through 126, inclusive,
                    // MAY be represented as the US-ASCII characters
                    piece = ((char)b).ToString();
                }
                else if (b == 9 || b == 32)
                {
                    // (Space and tab) Octets with decimal values of 9 and 32 MAY be represented as US-ASCII SPACE and TAB characters, respectively,
                    // except when they appear at the end of an encoded line.
                    if (spaceOrTabAtLineEnd)
                    {
                        piece = "=" + b.ToString("X2");
                    }
                    else
                    {
                        piece = ((char)b).ToString();
                    }
                }
                else
                {
                    //  (General 8bit representation) Any octet, except a CR or LF that is part of a CRLF line break of the canonical (standard) form of the data being encoded,
                    //  may be represented by an "=" followed by a two digit hexadecimal representation of the octet's value.
                    piece = "=" + b.ToString("X2");
                }

                // soft line break
                if (currentLineLength + piece.Length > maxLineLength)
                {
                    sb.Append("=\r\n");
                    currentLineLength = 0;
                }
                else if (currentLineLength + piece.Length == maxLineLength && i + 1 < bytes.Length)
                {
                    sb.Append("=\r\n");
                    currentLineLength = 0;
                }

                sb.Append(piece);
                currentLineLength += piece.Length;
            }

            return PluginInvocationResult.OfSuccess(sb.ToString());
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddListParameter(ParameterKeys.Encoding, PluginEncoding.GetListItems(
                PluginEncoding.UTF8,
                PluginEncoding.UTF16BE,
                PluginEncoding.UTF16LE,
                PluginEncoding.ShiftJIS,
                PluginEncoding.EUCJP,
                PluginEncoding.ISO2022JP
            ));
        }
    }
}
