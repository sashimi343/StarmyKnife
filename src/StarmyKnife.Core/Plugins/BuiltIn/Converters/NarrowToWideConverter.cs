using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("Narrow to wide")]
    public class NarrowToWideConverter : PluginBase, IConverter
    {
        public class ParameterKeys
        {
            public const string ConvertSpace = "ConvertSpace";
            public const string ConvertNumbers = "ConvertNumbers";
            public const string ConvertAlphabet = "ConvertAlphabet";
            public const string ConvertAsciiSymbols = "ConvertAsciiSymbols";
            public const string ConvertKatakana = "ConvertKatakana";
        }

        private static readonly Regex RegexNarrowKatakana = new(@"[ｦ-ﾝ]ﾞ|[ｦ-ﾝ]ﾟ|[ｦ-ﾝ]", RegexOptions.Compiled);

        private static readonly Dictionary<char, char> ConversionMap_AsciiSymbols = new()
        {
            { '!', '！' },
            { '"', '＂' },
            { '#', '＃' },
            { '$', '＄' },
            { '%', '％' },
            { '&', '＆' },
            { '\'', '＇' },
            { '(', '（' },
            { ')', '）' },
            { '*', '＊' },
            { '+', '＋' },
            { ',', '，' },
            { '-', '－' },
            { '.', '．' },
            { '/', '／' },
            { ':', '：' },
            { ';', '；' },
            { '<', '＜' },
            { '=', '＝' },
            { '>', '＞' },
            { '?', '？' },
            { '@', '＠' },
            { '[', '［' },
            { '\\', '＼' },
            { ']', '］' },
            { '^', '＾' },
            { '_', '＿' },
            { '`', '｀' },
            { '{', '｛' },
            { '|', '｜' },
            { '}', '｝' },
            { '~', '～' }
        };

        private static readonly Dictionary<string, string> ConversionMap_Katakana = new()
        {
            { "ｱ","ア" },
            { "ｧ","ァ" },
            { "ｨ","ィ" },
            { "ｲ","イ" },
            { "ｩ","ゥ" },
            { "ｳ","ウ" },
            { "ｪ","ェ" },
            { "ｴ","エ" },
            { "ｫ","ォ" },
            { "ｵ","オ" },
            { "ｶ","カ" },
            { "ｶﾞ","ガ" },
            { "ｷ","キ" },
            { "ｷﾞ","ギ" },
            { "ｸ","ク" },
            { "ｸﾞ","グ" },
            { "ｹ","ケ" },
            { "ｹﾞ","ゲ" },
            { "ｺ","コ" },
            { "ｺﾞ","ゴ" },
            { "ｻ","サ" },
            { "ｻﾞ","ザ" },
            { "ｼ","シ" },
            { "ｼﾞ","ジ" },
            { "ｽ","ス" },
            { "ｽﾞ","ズ" },
            { "ｾ","セ" },
            { "ｾﾞ","ゼ" },
            { "ｿ","ソ" },
            { "ｿﾞ","ゾ" },
            { "ﾀ","タ" },
            { "ﾀﾞ","ダ" },
            { "ﾁ","チ" },
            { "ﾁﾞ","ヂ" },
            { "ｯ","ッ" },
            { "ﾂ","ツ" },
            { "ﾂﾞ","ヅ" },
            { "ﾃ","テ" },
            { "ﾃﾞ","デ" },
            { "ﾄ","ト" },
            { "ﾄﾞ","ド" },
            { "ﾅ","ナ" },
            { "ﾆ","ニ" },
            { "ﾇ","ヌ" },
            { "ﾈ","ネ" },
            { "ﾉ","ノ" },
            { "ﾊ","ハ" },
            { "ﾊﾞ","バ" },
            { "ﾊﾟ","パ" },
            { "ﾋ","ヒ" },
            { "ﾋﾞ","ビ" },
            { "ﾋﾟ","ピ" },
            { "ﾌ","フ" },
            { "ﾌﾞ","ブ" },
            { "ﾌﾟ","プ" },
            { "ﾍ","ヘ" },
            { "ﾍﾞ","ベ" },
            { "ﾍﾟ","ペ" },
            { "ﾎ","ホ" },
            { "ﾎﾞ","ボ" },
            { "ﾎﾟ","ポ" },
            { "ﾏ","マ" },
            { "ﾐ","ミ" },
            { "ﾑ","ム" },
            { "ﾒ","メ" },
            { "ﾓ","モ" },
            { "ｬ","ャ" },
            { "ﾔ","ヤ" },
            { "ｭ","ュ" },
            { "ﾕ","ユ" },
            { "ｮ","ョ" },
            { "ﾖ","ヨ" },
            { "ﾗ","ラ" },
            { "ﾘ","リ" },
            { "ﾙ","ル" },
            { "ﾚ","レ" },
            { "ﾛ","ロ" },
            { "ヮ","ヮ" },
            { "ﾜ","ワ" },
            { "ヰ","ヰ" },
            { "ヱ","ヱ" },
            { "ｦ","ヲ" },
            { "ﾝ","ン" },
            { "ヴ","ヴ" },
            { "ヵ","ヵ" },
            { "ヶ","ヶ" },
            { "ｰ","ー" }
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
            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] == ' ')
                {
                    sb[i] = '　';
                }
            }

            return sb;
        }

        private StringBuilder ConvertNumbers(StringBuilder sb)
        {
            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] >= '0' && sb[i] <= '9')
                {
                    sb[i] = (char)(sb[i] - '0' + '０');
                }
            }

            return sb;
        }

        private StringBuilder ConvertAlphabet(StringBuilder sb)
        {
            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] >= 'A' && sb[i] <= 'Z')
                {
                    sb[i] = (char)(sb[i] - 'A' + 'Ａ');
                }
                else if (sb[i] >= 'a' && sb[i] <= 'z')
                {
                    sb[i] = (char)(sb[i] - 'a' + 'ａ');
                }
            }

            return sb;
        }

        private StringBuilder ConvertAsciiSymbols(StringBuilder sb)
        {
            for (int i = 0; i < sb.Length; i++)
            {
                if (ConversionMap_AsciiSymbols.TryGetValue(sb[i], out char wideChar))
                {
                    sb[i] = wideChar;
                }
            }

            return sb;
        }

        private StringBuilder ConvertKatakana(StringBuilder sb)
        {
            var output = RegexNarrowKatakana.Replace(sb.ToString(), match =>
            {
                if (ConversionMap_Katakana.TryGetValue(match.Value, out string wideChar))
                {
                    return wideChar;
                }
                return match.Value;
            });

            return new StringBuilder(output);
        }
    }
}
