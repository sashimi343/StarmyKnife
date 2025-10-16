using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using StarmyKnife.Core.Plugins.BuiltIn.Converters;
using StarmyKnife.PluginInterfaces;

namespace StarmyKnife.Core.Tests.xUnit.Converters
{
    public class FromQuotedPrintableConverterTests
    {
        [Fact]
        public void TestConvert_NonEncodedText_ReturnsSameText()
        {
            var input = "This is a simple text without any encoding.";
            var expectedOutput = input;
            var parameters = new KeyValuePair<string, object>[]
            {
                new(FromQuotedPrintableConverter.ParameterKeys.Encoding, PluginEncoding.UTF8)
            };
            var result = TestHelper.TestConvert<FromQuotedPrintableConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EncodedTextWithSoftLineBreaks_ReturnsDecodedText()
        {
            var input = "This is a long line that is encoded with soft line breaks=\r\n" +
                        "to ensure that it does not exceed the maximum line length of 76=\r\n" +
                        "characters. This should be decoded properly.";
            var expectedOutput = "This is a long line that is encoded with soft line breaksto ensure that it does not exceed the maximum line length of 76characters. This should be decoded properly.";
            var parameters = new KeyValuePair<string, object>[]
            {
                new(FromQuotedPrintableConverter.ParameterKeys.Encoding, PluginEncoding.UTF8)
            };

            var result = TestHelper.TestConvert<FromQuotedPrintableConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EncodedTextWithHexSequences_ReturnsDecodedText()
        {
            var input = "This is an encoded text with special characters: =C3=A9, =C3=B1, =C3=BC.";
            var expectedOutput = "This is an encoded text with special characters: é, ñ, ü.";
            var parameters = new KeyValuePair<string, object>[]
            {
                new(FromQuotedPrintableConverter.ParameterKeys.Encoding, PluginEncoding.UTF8)
            };

            var result = TestHelper.TestConvert<FromQuotedPrintableConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
