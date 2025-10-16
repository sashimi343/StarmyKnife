using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("To Hexdump")]
    public class ToHexdumpConverter : PluginBase, IConverter
    {
        public class ParameterKeys
        {
            public const string Encoding = "Encoding";
            public const string Width = "Width";
        }

        public enum Width
        {
            [Display(Name = "8")]
            Width8 = 8,
            [Display(Name = "16")]
            Width16 = 16,
            [Display(Name = "32")]
            Width32 = 32,
            [Display(Name = "64")]
            Width64 = 64,
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var encoding = parameters[ParameterKeys.Encoding].GetValue<Encoding>();
            var width = (int)(parameters[ParameterKeys.Width].GetValue<Width>());

            byte[] bytes;
            try
            {
                bytes = encoding.GetBytes(input);
            }
            catch (Exception ex)
            {
                return PluginInvocationResult.OfFailure($"Failed to convert input to byte array: {ex.Message}");
            }

            var sb = new StringBuilder();

            var address = 0;
            for (int i = 0; i < bytes.Length; i += width)
            {
                sb.Append(address.ToString("x8"));
                sb.Append("  ");
                var lineBytes = bytes.Skip(i).Take(width).ToArray();
                for (int j = 0; j < width; j++)
                {
                    if (j < lineBytes.Length)
                    {
                        sb.Append(lineBytes[j].ToString("x2"));
                    }
                    else
                    {
                        sb.Append("  ");
                    }
                    sb.Append(' ');

                    if (j % 8 == 7)
                    {
                        sb.Append(' ');
                    }
                }
                sb.Append('|');
                foreach (var b in lineBytes)
                {
                    if (b >= 0x20 && b <= 0x7E)
                    {
                        sb.Append((char)b);
                    }
                    else
                    {
                        sb.Append('.');
                    }
                }
                sb.AppendLine("|");
                address += width;
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
            configuration.AddListParameter<Width>(ParameterKeys.Width, Width.Width16);
        }
    }
}