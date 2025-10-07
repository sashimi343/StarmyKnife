using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StarmyKnife.Core.Plugins.BuiltIn.Converters;

using Xunit;

namespace StarmyKnife.Core.Tests.xUnit.Converters
{
    public class NarrowToWideConverterTests
    {
        [Fact]
        public void TestConvert_AllOptionsEnabled_ConvertsCorrectly()
        {
            var input = "Hello World! 123 ｱｲｳｴｵ";
            var expectedOutput = "Ｈｅｌｌｏ　Ｗｏｒｌｄ！　１２３　アイウエオ";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(NarrowToWideConverter.ParameterKeys.ConvertSpace, true),
                new KeyValuePair<string, object>(NarrowToWideConverter.ParameterKeys.ConvertNumbers, true),
                new KeyValuePair<string, object>(NarrowToWideConverter.ParameterKeys.ConvertAlphabet, true),
                new KeyValuePair<string, object>(NarrowToWideConverter.ParameterKeys.ConvertAsciiSymbols, true),
                new KeyValuePair<string, object>(NarrowToWideConverter.ParameterKeys.ConvertKatakana, true),
            };

            var result = TestHelper.TestConvert<NarrowToWideConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_NoOptionsEnabled_ReturnsOriginalString()
        {
            var input = "Hello World! 123 ｱｲｳｴｵ";
            var expectedOutput = input;
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(NarrowToWideConverter.ParameterKeys.ConvertSpace, false),
                new KeyValuePair<string, object>(NarrowToWideConverter.ParameterKeys.ConvertNumbers, false),
                new KeyValuePair<string, object>(NarrowToWideConverter.ParameterKeys.ConvertAlphabet, false),
                new KeyValuePair<string, object>(NarrowToWideConverter.ParameterKeys.ConvertAsciiSymbols, false),
                new KeyValuePair<string, object>(NarrowToWideConverter.ParameterKeys.ConvertKatakana, false),
            };

            var result = TestHelper.TestConvert<NarrowToWideConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_PartialOptionsEnabled_ConvertsAccordingly()
        {
            var input = "Hello World! 123 ｱｲｳｴｵ";
            var expectedOutput = "Ｈｅｌｌｏ　Ｗｏｒｌｄ!　123　ｱｲｳｴｵ"; // Only space and alphabet converted
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(NarrowToWideConverter.ParameterKeys.ConvertSpace, true),
                new KeyValuePair<string, object>(NarrowToWideConverter.ParameterKeys.ConvertNumbers, false),
                new KeyValuePair<string, object>(NarrowToWideConverter.ParameterKeys.ConvertAlphabet, true),
                new KeyValuePair<string, object>(NarrowToWideConverter.ParameterKeys.ConvertAsciiSymbols, false),
                new KeyValuePair<string, object>(NarrowToWideConverter.ParameterKeys.ConvertKatakana, false),
            };
            var result = TestHelper.TestConvert<NarrowToWideConverter>(input, parameters);
            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyString_ReturnsEmptyString()
        {
            var input = string.Empty;
            var expectedOutput = string.Empty;
            var result = TestHelper.TestConvert<NarrowToWideConverter>(input);
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
            var input = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var expectedOutput = "ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ";
            var result = TestHelper.TestConvert<NarrowToWideConverter>(input);
            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_AllNumbers_ConvertsCorrectly()
        {
            var input = "0123456789";
            var expectedOutput = "０１２３４５６７８９";
            var result = TestHelper.TestConvert<NarrowToWideConverter>(input);
            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_AllAsciiSymbols_ConvertsCorrectly()
        {
            var input = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            var expectedOutput = "！＂＃＄％＆＇（）＊＋，－．／：；＜＝＞？＠［＼］＾＿｀｛｜｝～";
            var result = TestHelper.TestConvert<NarrowToWideConverter>(input);
            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_AllKatakana_ConvertsCorrectly()
        {
            var input = "ｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜｦﾝｶﾞｷﾞｸﾞｹﾞｺﾞｻﾞｼﾞｽﾞｾﾞｿﾞﾀﾞﾁﾞﾂﾞﾃﾞﾄﾞﾊﾞﾋﾞﾌﾞﾍﾞﾎﾞﾊﾟﾋﾟﾌﾟﾍﾟﾎﾟｧｨｩｪｫｯｬｭｮ";
            var expectedOutput = "アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲンガギグゲゴザジズゼゾダヂヅデドバビブベボパピプペポァィゥェォッャュョ";
            var result = TestHelper.TestConvert<NarrowToWideConverter>(input);
            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
