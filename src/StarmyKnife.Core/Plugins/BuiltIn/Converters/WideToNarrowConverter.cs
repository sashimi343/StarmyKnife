using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("Wide to narrow")]
    public class WideToNarrowConverter : PluginBase, IConverter
    {
        public class ParameterKeys
        {
            public const string ConvertSpace = "ConvertSpace";
            public const string ConvertNumbers = "ConvertNumbers";
            public const string ConvertAlphabet = "ConvertAlphabet";
            public const string ConvertAsciiSymbols = "ConvertAsciiSymbols";
            public const string ConvertKatakana = "ConvertKatakana";
        }

        private static readonly Dictionary<char, char> ConversionMap_AsciiSymbols = new()
        {
            { '！', '!' },
            { '＂', '"' },
            { '＃', '#' },
            { '＄', '$' },
            { '％', '%' },
            { '＆', '&' },
            { '＇', '\'' },
            { '（', '(' },
            { '）', ')' },
            { '＊', '*' },
            { '＋', '+' },
            { '，', ',' },
            { '－', '-' },
            { '．', '.' },
            { '／', '/' },
            { '：', ':' },
            { '；', ';' },
            { '＜', '<' },
            { '＝', '=' },
            { '＞', '>' },
            { '？', '?' },
            { '＠', '@' },
            { '［', '[' },
            { '＼', '\\' },
            { '］', ']' },
            { '＾', '^' },
            { '＿', '_' },
            { '｀', '`' },
            { '｛', '{' },
            { '｜', '|' },
            { '｝', '}' },
            { '～', '~' }
        };

        private static readonly Dictionary<string, string> ConversionMap_Katakana = new()
        {
            { "ア","ｱ" },
            { "ァ","ｧ" },
            { "ィ","ｨ" },
            { "イ","ｲ" },
            { "ゥ","ｩ" },
            { "ウ","ｳ" },
            { "ェ","ｪ" },
            { "エ","ｴ" },
            { "ォ","ｫ" },
            { "オ","ｵ" },
            { "カ","ｶ" },
            { "ガ","ｶﾞ" },
            { "キ","ｷ" },
            { "ギ","ｷﾞ" },
            { "ク","ｸ" },
            { "グ","ｸﾞ" },
            { "ケ","ｹ" },
            { "ゲ","ｹﾞ" },
            { "コ","ｺ" },
            { "ゴ","ｺﾞ" },
            { "サ","ｻ" },
            { "ザ","ｻﾞ" },
            { "シ","ｼ" },
            { "ジ","ｼﾞ" },
            { "ス","ｽ" },
            { "ズ","ｽﾞ" },
            { "セ","ｾ" },
            { "ゼ","ｾﾞ" },
            { "ソ","ｿ" },
            { "ゾ","ｿﾞ" },
            { "タ","ﾀ" },
            { "ダ","ﾀﾞ" },
            { "チ","ﾁ" },
            { "ヂ","ﾁﾞ" },
            { "ッ","ｯ" },
            { "ツ","ﾂ" },
            { "ヅ","ﾂﾞ" },
            { "テ","ﾃ" },
            { "デ","ﾃﾞ" },
            { "ト","ﾄ" },
            { "ド","ﾄﾞ" },
            { "ナ","ﾅ" },
            { "ニ","ﾆ" },
            { "ヌ","ﾇ" },
            { "ネ","ﾈ" },
            { "ノ","ﾉ" },
            { "ハ","ﾊ" },
            { "バ","ﾊﾞ" },
            { "パ","ﾊﾟ" },
            { "ヒ","ﾋ" },
            { "ビ","ﾋﾞ" },
            { "ピ","ﾋﾟ" },
            { "フ","ﾌ" },
            { "ブ","ﾌﾞ" },
            { "プ","ﾌﾟ" },
            { "ヘ","ﾍ" },
            { "ベ","ﾍﾞ" },
            { "ペ","ﾍﾟ" },
            { "ホ","ﾎ" },
            { "ボ","ﾎﾞ" },
            { "ポ","ﾎﾟ" },
            { "マ","ﾏ" },
            { "ミ","ﾐ" },
            { "ム","ﾑ" },
            { "メ","ﾒ" },
            { "モ","ﾓ" },
            { "ャ","ｬ" },
            { "ヤ","ﾔ" },
            { "ュ","ｭ" },
            { "ユ","ﾕ" },
            { "ョ","ｮ" },
            { "ヨ","ﾖ" },
            { "ラ","ﾗ" },
            { "リ","ﾘ" },
            { "ル","ﾙ" },
            { "レ","ﾚ" },
            { "ロ","ﾛ" },
            { "ヮ","ヮ" },
            { "ワ","ﾜ" },
            { "ヰ","ヰ" },
            { "ヱ","ヱ" },
            { "ヲ","ｦ" },
            { "ン","ﾝ" },
            { "ヴ","ヴ" },
            { "ヵ","ヵ" },
            { "ヶ","ヶ" },
            { "ー","ｰ" }
        };

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            bool convertSpace = parameters[ParameterKeys.ConvertSpace].GetValue<bool>();
            bool convertNumbers = parameters[ParameterKeys.ConvertNumbers].GetValue<bool>();
            bool convertAlphabet = parameters[ParameterKeys.ConvertAlphabet].GetValue<bool>();
            bool convertAsciiSymbols = parameters[ParameterKeys.ConvertAsciiSymbols].GetValue<bool>();
            bool convertKatakana = parameters[ParameterKeys.ConvertKatakana].GetValue<bool>();

            var sb = new StringBuilder(input);

            sb = convertSpace ? ConvertSpace(sb) : sb;
            sb = convertNumbers ? ConvertNumbers(sb) : sb;
            sb = convertAlphabet ? ConvertAlphabet(sb) : sb;
            sb = convertAsciiSymbols ? ConvertAsciiSymbols(sb) : sb;
            sb = convertKatakana ? ConvertKatakana(sb) : sb;

            return PluginInvocationResult.OfSuccess(sb.ToString());
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddFlagParameter(ParameterKeys.ConvertSpace, true);
            configuration.AddFlagParameter(ParameterKeys.ConvertNumbers, true);
            configuration.AddFlagParameter(ParameterKeys.ConvertAlphabet, true);
            configuration.AddFlagParameter(ParameterKeys.ConvertAsciiSymbols, true);
            configuration.AddFlagParameter(ParameterKeys.ConvertKatakana, true);
        }

        private StringBuilder ConvertSpace(StringBuilder sb)
        {
            return sb.Replace("　", " ");
        }

        private StringBuilder ConvertNumbers(StringBuilder sb)
        {
            for (char c = '０'; c <= '９'; c++)
            {
                sb.Replace(c, (char)(c - '０' + '0'));
            }
            return sb;
        }

        private StringBuilder ConvertAlphabet(StringBuilder sb)
        {
            for (char c = 'Ａ'; c <= 'Ｚ'; c++)
            {
                sb.Replace(c, (char)(c - 'Ａ' + 'A'));
            }
            for (char c = 'ａ'; c <= 'ｚ'; c++)
            {
                sb.Replace(c, (char)(c - 'ａ' + 'a'));
            }
            return sb;
        }

        private StringBuilder ConvertAsciiSymbols(StringBuilder sb)
        {
            foreach (var kvp in ConversionMap_AsciiSymbols)
            {
                sb.Replace(kvp.Key, kvp.Value);
            }
            return sb;
        }

        private StringBuilder ConvertKatakana(StringBuilder sb)
        {
            foreach (var kvp in ConversionMap_Katakana)
            {
                sb.Replace(kvp.Key, kvp.Value);
            }
            return sb;
        }
    }
}
