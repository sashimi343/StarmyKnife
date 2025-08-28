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
    public class HtmlEncodingConverterTests
    {
        [Fact]
        public void TestConvert_SpecialCharacters_ReturnsEncodedString()
        {
            var input = "Hello & welcome to the world of <HTML>!";
            var expectedOutput = "Hello &amp; welcome to the world of &lt;HTML&gt;!";

            var result = TestHelper.TestConvert<HtmlEncodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_Quotes_ReturnsEncodedString()
        {
            var input = "She said, \"It's a beautiful day!\"";
            var expectedOutput = "She said, &quot;It&#39;s a beautiful day!&quot;";

            var result = TestHelper.TestConvert<HtmlEncodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_ConvertToNumeric_ReturnsNumericEncodedString()
        {
            var input = "Hello & welcome to the world of <HTML>!";
            var expectedOutput = "Hello &#38; welcome to the world of &#60;HTML&#62;!";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(HtmlEncodingConverter.ParameterKeys.EncodingMode, HtmlEncodingConverter.EncodingMode.ToNumericEntities)
            };

            var result = TestHelper.TestConvert<HtmlEncodingConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_ConvertToHex_ReturnsHexEncodedString()
        {
            var input = "Hello & welcome to the world of <HTML>!";
            var expectedOutput = "Hello &#x26; welcome to the world of &#x3C;HTML&#x3E;!";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(HtmlEncodingConverter.ParameterKeys.EncodingMode, HtmlEncodingConverter.EncodingMode.ToHexEntities)
            };

            var result = TestHelper.TestConvert<HtmlEncodingConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_AllCharactersToNumeric_ReturnsFullyNumericEncodedString()
        {
            var input = "ABC";
            var expectedOutput = "&#65;&#66;&#67;";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(HtmlEncodingConverter.ParameterKeys.EncodingMode, HtmlEncodingConverter.EncodingMode.ToNumericEntities),
                new KeyValuePair<string, object>(HtmlEncodingConverter.ParameterKeys.ConvertAllCharacters, true)
            };

            var result = TestHelper.TestConvert<HtmlEncodingConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_NoSpecialCharacters_ReturnsOriginalString()
        {
            var input = "This string has no special characters.";
            var expectedOutput = "This string has no special characters.";

            var result = TestHelper.TestConvert<HtmlEncodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyString_ReturnsEmptyString()
        {
            var input = string.Empty;
            var expectedOutput = string.Empty;
            var result = TestHelper.TestConvert<HtmlEncodingConverter>(input);
            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
