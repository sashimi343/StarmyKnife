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
    public class UrlEncodingConverterTests
    {
        [Fact]
        public void TestConvert_PlainText_ReturnsUrlEncodedText()
        {
            var input = "Hello, 世界!";
            var expectedOutput = "Hello%2C+%E4%B8%96%E7%95%8C!";

            var result = TestHelper.TestConvert<UrlEncodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EscapeAllSpecialChars_ReturnsUrlEncodedText()
        {
            var input = "Hello, 世界!";
            var expectedOutput = "Hello%2C%20%E4%B8%96%E7%95%8C%21";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(UrlEncodingConverter.ParameterKeys.EscapeAllSpecialChars, true),
            };

            var result = TestHelper.TestConvert<UrlEncodingConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyString_ReturnsEmptyString()
        {
            var input = string.Empty;
            var expectedOutput = string.Empty;

            var result = TestHelper.TestConvert<UrlEncodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
