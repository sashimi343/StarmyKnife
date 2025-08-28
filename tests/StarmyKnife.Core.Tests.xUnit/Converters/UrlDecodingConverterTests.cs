using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using StarmyKnife.PluginInterfaces;
using StarmyKnife.Core.Plugins.BuiltIn.Converters;

namespace StarmyKnife.Core.Tests.xUnit.Converters
{
    public class UrlDecodingConverterTests
    {
        [Fact]
        public void TestConvert_UrlEncodedText_ReturnsDecodedText()
        {
            var input = "Hello%2C%20%E4%B8%96%E7%95%8C%21";
            var expectedOutput = "Hello, 世界!";

            var result = TestHelper.TestConvert<UrlDecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyString_ReturnsEmptyString()
        {
            var input = string.Empty;
            var expectedOutput = string.Empty;

            var result = TestHelper.TestConvert<UrlDecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_InvalidUrlEncodedText_ReturnsInputAsIs()
        {
            var input = "Hello%2GWorld"; // %2G is not a valid URL encoding
            var expectedOutput = "Hello%2GWorld";
            var result = TestHelper.TestConvert<UrlDecodingConverter>(input);
            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
