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
    public class ToQuotedPrintableConverterTests
    {
        [Fact]
        public void TestConvert_AsciiString_ReturnsOriginalString()
        {
            var input = "Hello World!";
            var expectedOutput = "Hello World!";
            var parameters = new KeyValuePair<string, object>[]
            {
                new(ToQuotedPrintableConverter.ParameterKeys.Encoding, PluginEncoding.UTF8)
            };

            var result = TestHelper.TestConvert<ToQuotedPrintableConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_NonAsciiString_ReturnsQuotedPrintable()
        {
            var input = "Café";
            var expectedOutput = "Caf=C3=A9";
            var parameters = new KeyValuePair<string, object>[]
            {
                new(ToQuotedPrintableConverter.ParameterKeys.Encoding, PluginEncoding.UTF8)
            };

            var result = TestHelper.TestConvert<ToQuotedPrintableConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_LongString_ReturnsFoldedQuotedPrintable()
        {
            var input = new string('A', 100);
            var expectedOutput = new string('A', 75) + "=\r\n" + new string('A', 25);
            var parameters = new KeyValuePair<string, object>[]
            {
                new(ToQuotedPrintableConverter.ParameterKeys.Encoding, PluginEncoding.UTF8)
            };

            var result = TestHelper.TestConvert<ToQuotedPrintableConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_LongStringWithNonAscii_ReturnsFoldedQuotedPrintable()
        {
            var input = new string('A', 70) + "Café" + new string('B', 70);
            var expectedOutput = new string('A', 70) + "Caf=\r\n" + "=C3=A9" + new string('B', 70);
            var parameters = new KeyValuePair<string, object>[]
            {
                new(ToQuotedPrintableConverter.ParameterKeys.Encoding, PluginEncoding.UTF8)
            };

            var result = TestHelper.TestConvert<ToQuotedPrintableConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_StringWithLineBreaks_ReturnsQuotedPrintableWithCRLF()
        {
            var input = "Hello\r\nWorld!\nThis is a test.\rEnd of test.";
            var expectedOutput = "Hello\r\nWorld!\r\nThis is a test.\r\nEnd of test.";
            var parameters = new KeyValuePair<string, object>[]
            {
                new(ToQuotedPrintableConverter.ParameterKeys.Encoding, PluginEncoding.UTF8)
            };

            var result = TestHelper.TestConvert<ToQuotedPrintableConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_StringWithTrailingSpace_ReturnsEncodedTrailingSpace()
        {
            var input = "Hello World! ";
            var expectedOutput = "Hello World!=20";
            var parameters = new KeyValuePair<string, object>[]
            {
                new(ToQuotedPrintableConverter.ParameterKeys.Encoding, PluginEncoding.UTF8)
            };

            var result = TestHelper.TestConvert<ToQuotedPrintableConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_InvalidEncoding_ReturnsFailure()
        {
            var input = "Café";
            var parameters = new KeyValuePair<string, object>[]
            {
                new(ToQuotedPrintableConverter.ParameterKeys.Encoding, PluginEncoding.ShiftJIS)
            };

            var result = TestHelper.TestConvert<ToQuotedPrintableConverter>(input, parameters);

            Assert.False(result.Success);
            Assert.NotEmpty(result.Errors);
        }
    }
}
