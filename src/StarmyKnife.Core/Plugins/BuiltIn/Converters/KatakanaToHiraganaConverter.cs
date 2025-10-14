using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("Katakana to Hiragana")]
    public class KatakanaToHiraganaConverter : PluginBase, IConverter
    {
        private static readonly Dictionary<string, string> ConversionMap = new()
        {
            {"ア","あ"},
            {"ァ","ぁ"},
            {"ィ","ぃ"},
            {"イ","い"},
            {"ゥ","ぅ"},
            {"ウ","う"},
            {"ェ","ぇ"},
            {"エ","え"},
            {"ォ","ぉ"},
            {"オ","お"},
            {"カ","か"},
            {"ガ","が"},
            {"キ","き"},
            {"ギ","ぎ"},
            {"ク","く"},
            {"グ","ぐ"},
            {"ケ","け"},
            {"ゲ","げ"},
            {"コ","こ"},
            {"ゴ","ご"},
            {"サ","さ"},
            {"ザ","ざ"},
            {"シ","し"},
            {"ジ","じ"},
            {"ス","す"},
            {"ズ","ず"},
            {"セ","せ"},
            {"ゼ","ぜ"},
            {"ソ","そ"},
            {"ゾ","ぞ"},
            {"タ","た"},
            {"ダ","だ"},
            {"チ","ち"},
            {"ヂ","ぢ"},
            {"ッ","っ"},
            {"ツ","つ"},
            {"ヅ","づ"},
            {"テ","て"},
            {"デ","で"},
            {"ト","と"},
            {"ド","ど"},
            {"ナ","な"},
            {"ニ","に"},
            {"ヌ","ぬ"},
            {"ネ","ね"},
            {"ノ","の"},
            {"ハ","は"},
            {"バ","ば"},
            {"パ","ぱ"},
            {"ヒ","ひ"},
            {"ビ","び"},
            {"ピ","ぴ"},
            {"フ","ふ"},
            {"ブ","ぶ"},
            {"プ","ぷ"},
            {"ヘ","へ"},
            {"ベ","べ"},
            {"ペ","ぺ"},
            {"ホ","ほ"},
            {"ボ","ぼ"},
            {"ポ","ぽ"},
            {"マ","ま"},
            {"ミ","み"},
            {"ム","む"},
            {"メ","め"},
            {"モ","も"},
            {"ャ","ゃ"},
            {"ヤ","や"},
            {"ュ","ゅ"},
            {"ユ","ゆ"},
            {"ョ","ょ"},
            {"ヨ","よ"},
            {"ラ","ら"},
            {"リ","り"},
            {"ル","る"},
            {"レ","れ"},
            {"ロ","ろ"},
            {"ヮ","ゎ"},
            {"ワ","わ"},
            {"ヰ","ゐ"},
            {"ヱ","ゑ"},
            {"ヲ","を"},
            {"ン","ん"},
            {"ヴ","ゔ"},
        };

        private readonly IConverter _narrowToWideConverter;
        private readonly PluginParameterCollection _narrowToWideParameter;

        public KatakanaToHiraganaConverter()
        {
            _narrowToWideConverter = new NarrowToWideConverter();
            _narrowToWideParameter = _narrowToWideConverter.GetParametersSchema();

            foreach (var key in _narrowToWideParameter.Keys)
            {
                if (key == NarrowToWideConverter.ParameterKeys.ConvertKatakana || key == NarrowToWideConverter.ParameterKeys.ConvertSpace)
                {
                    _narrowToWideParameter[key].SetValue(true);
                }
                else
                {
                    _narrowToWideParameter[key].SetValue(false);
                }
            }
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var wideKatakanaResult = _narrowToWideConverter.Convert(input, _narrowToWideParameter);
            if (!wideKatakanaResult.Success)
            {
                return PluginInvocationResult.OfFailure("Cannot convert narrow katakana to wide");
            }

            var sb = new StringBuilder(wideKatakanaResult.Value);
            foreach (var kvp in ConversionMap)
            {
                sb.Replace(kvp.Key, kvp.Value);
            }

            return PluginInvocationResult.OfSuccess(sb.ToString());
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            // No parameters
        }
    }
}
