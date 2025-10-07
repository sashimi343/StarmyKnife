using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StarmyKnife.Core.Plugins.BuiltIn.Converters;
using Xunit;

namespace StarmyKnife.Core.Tests.xUnit.Converters
{
    public class WideToNarrowConverterTests
    {
        [Fact]
        public void TestConvert_AllOptionsEnabled_ConvertsCorrectly()
        {
            var input = "Ｈｅｌｌｏ　Ｗｏｒｌｄ！　１２３　アイウエオ";
            var expectedOutput = "Hello World! 123 ｱｲｳｴｵ";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(WideToNarrowConverter.ParameterKeys.ConvertSpace, true),
                new KeyValuePair<string, object>(WideToNarrowConverter.ParameterKeys.ConvertNumbers, true),
                new KeyValuePair<string, object>(WideToNarrowConverter.ParameterKeys.ConvertAlphabet, true),
                new KeyValuePair<string, object>(WideToNarrowConverter.ParameterKeys.ConvertAsciiSymbols, true),
                new KeyValuePair<string, object>(WideToNarrowConverter.ParameterKeys.ConvertKatakana, true),
            };
            
            var result = TestHelper.TestConvert<WideToNarrowConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_NoOptionsEnabled_ReturnsOriginalString()
        {
            var input = "Ｈｅｌｌｏ　Ｗｏｒｌｄ！　１２３　アイウエオ";
            var expectedOutput = input;
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(WideToNarrowConverter.ParameterKeys.ConvertSpace, false),
                new KeyValuePair<string, object>(WideToNarrowConverter.ParameterKeys.ConvertNumbers, false),
                new KeyValuePair<string, object>(WideToNarrowConverter.ParameterKeys.ConvertAlphabet, false),
                new KeyValuePair<string, object>(WideToNarrowConverter.ParameterKeys.ConvertAsciiSymbols, false),
                new KeyValuePair<string, object>(WideToNarrowConverter.ParameterKeys.ConvertKatakana, false),
            };

            var result = TestHelper.TestConvert<WideToNarrowConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_PartialOptionsEnabled_ConvertsAccordingly()
        {
            var input = "Ｈｅｌｌｏ　Ｗｏｒｌｄ！　１２３　アイウエオ";
            var expectedOutput = "Hello World！ １２３ アイウエオ"; // Only space and alphabet converted
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(WideToNarrowConverter.ParameterKeys.ConvertSpace, true),
                new KeyValuePair<string, object>(WideToNarrowConverter.ParameterKeys.ConvertNumbers, false),
                new KeyValuePair<string, object>(WideToNarrowConverter.ParameterKeys.ConvertAlphabet, true),
                new KeyValuePair<string, object>(WideToNarrowConverter.ParameterKeys.ConvertAsciiSymbols, false),
                new KeyValuePair<string, object>(WideToNarrowConverter.ParameterKeys.ConvertKatakana, false),
            };

            var result = TestHelper.TestConvert<WideToNarrowConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyString_ReturnsEmptyString()
        {
            var input = "";
            var expectedOutput = "";

            var result = TestHelper.TestConvert<WideToNarrowConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_StringWithoutConvertibleCharacters_ReturnsOriginalString()
        {
            var input = "こんにちは"; // Japanese Hiragana, no narrow characters to convert
            var expectedOutput = input;

            var result = TestHelper.TestConvert<NarrowToWideConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_AllAlphabets_ConvertsCorrectly()
        {
            var input = "ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ";
            var expectedOutput = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            var result = TestHelper.TestConvert<WideToNarrowConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_AllNumbers_ConvertsCorrectly()
        {
            var input = "０１２３４５６７８９";
            var expectedOutput = "0123456789";

            var result = TestHelper.TestConvert<WideToNarrowConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_AllAsciiSymbols_ConvertsCorrectly()
        {
            var expectedOutput = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            var input = "！＂＃＄％＆＇（）＊＋，－．／：；＜＝＞？＠［＼］＾＿｀｛｜｝～";

            var result = TestHelper.TestConvert<WideToNarrowConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_AllKatakana_ConvertsCorrectly()
        {
            var input = "アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲンガギグゲゴザジズゼゾダヂヅデドバビブベボパピプペポァィゥェォッャュョ";
            var expectedOutput = "ｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜｦﾝｶﾞｷﾞｸﾞｹﾞｺﾞｻﾞｼﾞｽﾞｾﾞｿﾞﾀﾞﾁﾞﾂﾞﾃﾞﾄﾞﾊﾞﾋﾞﾌﾞﾍﾞﾎﾞﾊﾟﾋﾟﾌﾟﾍﾟﾎﾟｧｨｩｪｫｯｬｭｮ";

            var result = TestHelper.TestConvert<WideToNarrowConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
