using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("Hiragana to Katakana")]
    public class HiraganaToKatakanaConverter : PluginBase, IConverter
    {
        public class ParameterKeys
        {
            public const string OutputFormat = "OutputFormat";
        }

        public enum OutputFormat
        {
            FullWidth,
            HalfWidth
        }

        private static readonly Dictionary<string, string> ConversionMap = new()
        {
            {"あ","ア"},
            {"ぁ","ァ"},
            {"ぃ","ィ"},
            {"い","イ"},
            {"ぅ","ゥ"},
            {"う","ウ"},
            {"ぇ","ェ"},
            {"え","エ"},
            {"ぉ","ォ"},
            {"お","オ"},
            {"か","カ"},
            {"が","ガ"},
            {"き","キ"},
            {"ぎ","ギ"},
            {"く","ク"},
            {"ぐ","グ"},
            {"け","ケ"},
            {"げ","ゲ"},
            {"こ","コ"},
            {"ご","ゴ"},
            {"さ","サ"},
            {"ざ","ザ"},
            {"し","シ"},
            {"じ","ジ"},
            {"す","ス"},
            {"ず","ズ"},
            {"せ","セ"},
            {"ぜ","ゼ"},
            {"そ","ソ"},
            {"ぞ","ゾ"},
            {"た","タ"},
            {"だ","ダ"},
            {"ち","チ"},
            {"ぢ","ヂ"},
            {"っ","ッ"},
            {"つ","ツ"},
            {"づ","ヅ"},
            {"て","テ"},
            {"で","デ"},
            {"と","ト"},
            {"ど","ド"},
            {"な","ナ"},
            {"に","ニ"},
            {"ぬ","ヌ"},
            {"ね","ネ"},
            {"の","ノ"},
            {"は","ハ"},
            {"ば","バ"},
            {"ぱ","パ"},
            {"ひ","ヒ"},
            {"び","ビ"},
            {"ぴ","ピ"},
            {"ふ","フ"},
            {"ぶ","ブ"},
            {"ぷ","プ"},
            {"へ","ヘ"},
            {"べ","ベ"},
            {"ぺ","ペ"},
            {"ほ","ホ"},
            {"ぼ","ボ"},
            {"ぽ","ポ"},
            {"ま","マ"},
            {"み","ミ"},
            {"む","ム"},
            {"め","メ"},
            {"も","モ"},
            {"ゃ","ャ"},
            {"や","ヤ"},
            {"ゅ","ュ"},
            {"ゆ","ユ"},
            {"ょ","ョ"},
            {"よ","ヨ"},
            {"ら","ラ"},
            {"り","リ"},
            {"る","ル"},
            {"れ","レ"},
            {"ろ","ロ"},
            {"ゎ","ヮ"},
            {"わ","ワ"},
            {"ゐ","ヰ"},
            {"ゑ","ヱ"},
            {"を","ヲ"},
            {"ん","ン"},
            {"う゛", "ヴ" },
            {"ゔ","ヴ"},
        };

        private readonly IConverter _wideToNarrowConverter;
        private readonly PluginParameterCollection _wideToNarrowParameter;

        public HiraganaToKatakanaConverter()
        {
            _wideToNarrowConverter = new WideToNarrowConverter();
            _wideToNarrowParameter = _wideToNarrowConverter.GetParametersSchema();

            foreach (var key in _wideToNarrowParameter.Keys)
            {
                if (key == WideToNarrowConverter.ParameterKeys.ConvertKatakana || key == WideToNarrowConverter.ParameterKeys.ConvertSpace)
                {
                    _wideToNarrowParameter[key].SetValue(true);
                }
                else
                {
                    _wideToNarrowParameter[key].SetValue(false);
                }
            }
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var outputFormat = parameters[ParameterKeys.OutputFormat].GetValue<OutputFormat>();

            var fullWidthKatakana = ConvertToFullWidth(input);

            if (outputFormat == OutputFormat.FullWidth)
            {
                return PluginInvocationResult.OfSuccess(fullWidthKatakana);
            }
            else
            {
                return _wideToNarrowConverter.Convert(fullWidthKatakana, _wideToNarrowParameter);
            }
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddListParameter<OutputFormat>(ParameterKeys.OutputFormat, OutputFormat.FullWidth);
        }

        private string ConvertToFullWidth(string input)
        {
            var sb = new StringBuilder(input);
            foreach (var kvp in ConversionMap)
            {
                sb.Replace(kvp.Key, kvp.Value);
            }
            return sb.ToString();
        }
    }
}
