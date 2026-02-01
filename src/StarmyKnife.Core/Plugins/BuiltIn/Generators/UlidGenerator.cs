using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Generators
{
    [StarmyKnifePlugin("ULID")]
    public class UlidGenerator : PluginBase, IGenerator
    {
        public class ParameterKeys
        {
            public const string UseUUIDFormat = "UseUUIDFormat";
        }

        private static readonly char[] CrockfordBase32Chars = "0123456789ABCDEFGHJKMNPQRSTVWXYZ".ToCharArray();
        private static readonly int[] CrockfordBase32DecodeTable = new int[128];
        private static readonly int TimestampLength = 10;
        private static readonly int EntropyLength = 16;
        private static readonly int TotalLength = TimestampLength + EntropyLength;

        static UlidGenerator()
        {
            for (int i = 0; i < CrockfordBase32DecodeTable.Length; i++)
            {
                CrockfordBase32DecodeTable[i] = -1;
            }
            for (int i = 0; i < CrockfordBase32Chars.Length; i++)
            {
                CrockfordBase32DecodeTable[CrockfordBase32Chars[i]] = i;
            }
        }

        public string Generate(PluginParameterCollection parameters)
        {
            var useUUIDFormat = parameters[ParameterKeys.UseUUIDFormat].GetValue<bool>();

            var ulidChars = new char[TotalLength];

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            EncodeTimestamp(timestamp, ulidChars);

            var random = new Random();
            var entropyBytes = new byte[EntropyLength / 2];
            random.NextBytes(entropyBytes);
            EncodeEntropy(entropyBytes, ulidChars);

            if (useUUIDFormat)
            {
                Span<byte> bytes = stackalloc byte[16];

                int bitBuffer = 0;
                int bitCount = 0;
                int byteIndex = 0;

                foreach (char c in ulidChars)
                {
                    int val = CrockfordBase32DecodeTable[c];
                    bitBuffer = (bitBuffer << 5) | val;
                    bitCount += 5;

                    if (bitCount >= 8)
                    {
                        bitCount -= 8;
                        bytes[byteIndex++] = (byte)((bitBuffer >> bitCount) & 0xFF);
                    }
                }

                return new Guid(bytes).ToString();
            }
            else
            {
                return new string(ulidChars);
            }
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddFlagParameter(ParameterKeys.UseUUIDFormat);
        }

        private void EncodeTimestamp(long timestamp, char[] ulidChars)
        {
            for (int i = TimestampLength - 1; i >= 0; i--)
            {
                ulidChars[i] = CrockfordBase32Chars[timestamp % 32];
                timestamp /= 32;
            }
        }

        private void EncodeEntropy(byte[] entropyBytes, char[] ulidChars)
        {
            for (int i = 0; i < entropyBytes.Length; i++)
            {
                ulidChars[TimestampLength + i * 2] = CrockfordBase32Chars[entropyBytes[i] >> 4];
                ulidChars[TimestampLength + i * 2 + 1] = CrockfordBase32Chars[entropyBytes[i] & 0x0F];
            }
        }
    }
}
