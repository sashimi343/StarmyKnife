using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using StarmyKnife.Core.Plugins;
using StarmyKnife.Core.Plugins.BuiltIn.Converters;

namespace StarmyKnife.Core.Tests.xUnit.Converters
{
    public class HexDecodingConverterTests
    {
        [Fact]
        public void TestConvert_InputWithNoPrefix_ReturnsDecodedString()
        {
            var input = "E38386E382B9E38388E381A7E38199";
            var expectedOutput = "テストです";

            var result = TestHelper.TestConvert<HexDecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_InputWithPercentPrefix_ReturnsDecodedString()
        {
            var input = "%E3%83%86%E3%82%B9%E3%83%88%E3%81%A7%E3%81%99";
            var expectedOutput = "テストです";

            var result = TestHelper.TestConvert<HexDecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_InputWithZeroXComma_ReturnsDecodedString()
        {
            var input = "0xE3,0x83,0x86,0xE3,0x82,0xB9,0xE3,0x83,0x88,0xE3,0x81,0xA7,0xE3,0x81,0x99";
            var expectedOutput = "テストです";

            var result = TestHelper.TestConvert<HexDecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_InputWithSemiColonSuffix_ReturnsDecodedString()
        {
            var input = "e3;83;86;e3;82;b9;e3;83;88;e3;81;a7;e3;81;99";
            var expectedOutput = "テストです";

            var result = TestHelper.TestConvert<HexDecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_InputWithCRLF_ReturnsDecodedString()
        {
            var input = string.Join("\r\n", ["E3", "83", "86", "E3", "82", "B9", "E3", "83", "88", "E3", "81", "A7", "E3", "81", "99"]);
            var expectedOutput = "テストです";

            var result = TestHelper.TestConvert<HexDecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_InputWithLF_ReturnsDecodedString()
        {
            var input = string.Join("\n", ["E3", "83", "86", "E3", "82", "B9", "E3", "83", "88", "E3", "81", "A7", "E3", "81", "99"]);
            var expectedOutput = "テストです";

            var result = TestHelper.TestConvert<HexDecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_SpecifyEncodingAndDelimiter_ReturnsDecodedString()
        {
            var input = "55 00 54 00 46 00 31 00 36 00 2d 00 4c 00 45 00 87 65 57 5b 17 52";
            var expectedOutput = "UTF16-LE文字列";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(HexDecodingConverter.ParameterKeys.Encoding, PluginEncoding.UTF16LE),
                new KeyValuePair<string, object>(HexDecodingConverter.ParameterKeys.Delimiter, HexDecodingConverter.DelimiterType.Space)
            };

            var result = TestHelper.TestConvert<HexDecodingConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_InputWithInvalidHexSequence_ReturnsError()
        {
            var input = "xxxxE38386E382B9E38388E381A7E38199";

            var result = TestHelper.TestConvert<HexDecodingConverter>(input);

            Assert.False(result.Success);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void TestConvert_InputWithInvalidDelimiter_ReturnsError()
        {
            var input = "E3,83,86,E3,82,B9,E3,83,88,E3,81,A7,E3,81,99";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(HexDecodingConverter.ParameterKeys.Delimiter, HexDecodingConverter.DelimiterType.Space)
            };

            var result = TestHelper.TestConvert<HexDecodingConverter>(input, parameters);

            Assert.False(result.Success);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void TestConvert_InputWithOddLength_ReturnsError()
        {
            var input = "E38386E382B9E38388E381A7E3819";  // Last byte is incomplete

            var result = TestHelper.TestConvert<HexDecodingConverter>(input);

            Assert.False(result.Success);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void TestConvert_EmptyInput_ReturnsEmptyString()
        {
            var input = "";
            var expectedOutput = "";

            var result = TestHelper.TestConvert<HexDecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_InputWithInvalidEncoding_ReturnsError()
        {
            var input = "55:54:46:38:e6:96:87:e5:ad:97:e5:88:97";       // UTF-8 encoded "UTF8文字列"
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(HexDecodingConverter.ParameterKeys.Encoding, PluginEncoding.ShiftJIS),
                new KeyValuePair<string, object>(HexDecodingConverter.ParameterKeys.Delimiter, HexDecodingConverter.DelimiterType.Colon)
            };

            var result = TestHelper.TestConvert<HexDecodingConverter>(input, parameters);

            Assert.False(result.Success);
            Assert.NotEmpty(result.Errors);
        }
    }
}
