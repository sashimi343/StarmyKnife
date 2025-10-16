using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("From Quoted Printable (QP decoding)")]
    public class FromQuotedPrintableConverter : PluginBase, IConverter
    {
        public class ParameterKeys
        {
            public const string Encoding = "Encoding";
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            // treat null as empty string
            input ??= string.Empty;

            var encoding = parameters[ParameterKeys.Encoding].GetValue<Encoding>();
            var outputBytes = new List<byte>();

            try
            {
                int i = 0;
                while (i < input.Length)
                {
                    char c = input[i];

                    if (c == '=')
                    {
                        // '=' at end -> invalid
                        if (i + 1 >= input.Length)
                        {
                            return PluginInvocationResult.OfFailure("Invalid quoted-printable sequence: trailing '='");
                        }

                        // Soft line break: "=\r\n", "=\n" or "=\r"
                        if (input[i + 1] == '\r')
                        {
                            if (i + 2 < input.Length && input[i + 2] == '\n')
                            {
                                i += 3;
                                continue;
                            }
                            else
                            {
                                i += 2;
                                continue;
                            }
                        }
                        if (input[i + 1] == '\n')
                        {
                            i += 2;
                            continue;
                        }

                        // Expect two hex digits after '='
                        if (i + 2 < input.Length && IsHex(input[i + 1]) && IsHex(input[i + 2]))
                        {
                            string hex = new string(new[] { input[i + 1], input[i + 2] });
                            byte b = System.Convert.ToByte(hex, 16);
                            outputBytes.Add(b);
                            i += 3;
                            continue;
                        }

                        return PluginInvocationResult.OfFailure($"Invalid quoted-printable escape at position {i}");
                    }

                    if (c == '\r')
                    {
                        // preserve CRLF if present, otherwise preserve CR
                        if (i + 1 < input.Length && input[i + 1] == '\n')
                        {
                            outputBytes.Add(0x0D);
                            outputBytes.Add(0x0A);
                            i += 2;
                        }
                        else
                        {
                            outputBytes.Add(0x0D);
                            i++;
                        }
                        continue;
                    }

                    if (c == '\n')
                    {
                        outputBytes.Add(0x0A);
                        i++;
                        continue;
                    }

                    // Ordinary literal character (should be ASCII in QP). Use raw byte.
                    outputBytes.Add((byte)c);
                    i++;
                }

                string decoded = encoding.GetString(outputBytes.ToArray());
                return PluginInvocationResult.OfSuccess(decoded);
            }
            catch (Exception e) when (e is FormatException || e is ArgumentException || e is DecoderFallbackException)
            {
                return PluginInvocationResult.OfFailure(e.Message);
            }
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

        private static bool IsHex(char c)
        {
            return (c >= '0' && c <= '9') ||
                   (c >= 'A' && c <= 'F') ||
                   (c >= 'a' && c <= 'f');
        }
    }
}
