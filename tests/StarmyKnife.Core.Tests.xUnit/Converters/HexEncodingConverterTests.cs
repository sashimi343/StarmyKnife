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
    public class HexEncodingConverterTests
    {
        [Fact]
        public void TestConvert_DefaultDelimiterAndEncoding_ReturnsEncodedString()
        {
            var input = "テストです";
            var expectedOutput = "E38386E382B9E38388E381A7E38199";

            var result = TestHelper.TestConvert<HexEncodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_SpecifyDelimiter_ReturnsEncodedStringWithSelectedDelimiter()
        {
            var input = "テストです";
            var expectedOutput = "E3 83 86 E3 82 B9 E3 83 88 E3 81 A7 E3 81 99";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(HexEncodingConverter.ParameterKeys.Delimiter, HexEncodingConverter.DelimiterType.Space)
            };

            var result = TestHelper.TestConvert<HexEncodingConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_SpecifyEncoding_ReturnsEncodedStringWithSelectedEncoding()
        {
            var input = "UTF16-LE文字列";
            var expectedOutput = @"0x55,0x00,0x54,0x00,0x46,0x00,0x31,0x00,0x36,0x00,0x2D,0x00,0x4C,0x00,0x45,0x00,0x87,0x65,0x57,0x5B,0x17,0x52";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(HexEncodingConverter.ParameterKeys.Encoding, PluginEncoding.UTF16LE),
                new KeyValuePair<string, object>(HexEncodingConverter.ParameterKeys.Delimiter, HexEncodingConverter.DelimiterType.ZeroXWithComma)
            };

            var result = TestHelper.TestConvert<HexEncodingConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_SpecifyBytesPerLine_ReturnsEncodedStringWithLineBreaks()
        {
            var input = "UTF16-LE文字列";
            var expectedOutput = string.Join(Environment.NewLine, ["%55%00%54%00","%46%00%31%00","%36%00%2D%00","%4C%00%45%00","%87%65%57%5B","%17%52"]);
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(HexEncodingConverter.ParameterKeys.Encoding, PluginEncoding.UTF16LE),
                new KeyValuePair<string, object>(HexEncodingConverter.ParameterKeys.Delimiter, HexEncodingConverter.DelimiterType.Percent),
                new KeyValuePair<string, object>(HexEncodingConverter.ParameterKeys.BytesPerLine, 4)
            };

            var result = TestHelper.TestConvert<HexEncodingConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyInput_ReturnsEmptyString()
        {
            var input = "";
            var expectedOutput = "";

            var result = TestHelper.TestConvert<HexEncodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_InputWithInvalidEncoding_ReturnsError()
        {
            var input = "𠮷野家";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(HexEncodingConverter.ParameterKeys.Encoding, PluginEncoding.ShiftJIS)
            };

            var result = TestHelper.TestConvert<HexEncodingConverter>(input, parameters);

            Assert.False(result.Success);
            Assert.NotEmpty(result.Errors);
        }
    }
}
