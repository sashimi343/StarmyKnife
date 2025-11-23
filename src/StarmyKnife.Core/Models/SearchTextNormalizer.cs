using System;
using System.Text;

namespace StarmyKnife.Core.Models
{
    public static class SearchTextNormalizer
    {
        private static readonly bool[] _removeAsciiSymbol = new bool[128];

        static SearchTextNormalizer()
        {
            const string symbols = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            foreach (var ch in symbols)
            {
                _removeAsciiSymbol[ch] = true;
            }
        }

        public static string Normalize(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var length = text.Length;
            var buffer = new char[length];
            var pos = 0;

            for (var i = 0; i < length; i++)
            {
                var c = text[i];

                if (char.IsWhiteSpace(c))
                {
                    continue;
                }

                if (c < 128 && _removeAsciiSymbol[c])
                {
                    continue;
                }

                buffer[pos++] = char.ToLowerInvariant(c);
            }

            return pos == length ? new string(buffer) : new string(buffer, 0, pos);
        }
    }
}
