using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Generators
{
    [StarmyKnifePlugin("ULID")]
    public class UlidGenerator : PluginBase, IGenerator
    {
        private static readonly char[] CrockfordBase32Chars = "0123456789ABCDEFGHJKMNPQRSTVWXYZ".ToCharArray();
        private static readonly int TimestampLength = 10;
        private static readonly int EntropyLength = 16;
        private static readonly int TotalLength = TimestampLength + EntropyLength;

        public string Generate(PluginParameterCollection parameters)
        {
            var ulidChars = new char[TotalLength];

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            EncodeTimestamp(timestamp, ulidChars);

            var random = new Random();
            var entropyBytes = new byte[EntropyLength / 2];
            random.NextBytes(entropyBytes);
            EncodeEntropy(entropyBytes, ulidChars);

            return new string(ulidChars);
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
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
