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
    public class HtmlDecodingConverterTests
    {
        [Fact]
        public void TestConvert_NamedEntities_ReturnsDecodedString()
        {
            var input = "Hello &amp; welcome to the world of &lt;HTML&gt;!";
            var expectedOutput = "Hello & welcome to the world of <HTML>!";

            var result = TestHelper.TestConvert<HtmlDecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_NumericEntities_ReturnsDecodedString()
        {
            var input = "The price is &#36;100 and the temperature is &#8451;25.";
            var expectedOutput = "The price is $100 and the temperature is ℃25.";

            var result = TestHelper.TestConvert<HtmlDecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_HexEntities_ReturnsDecodedString()
        {
            var input = "Hex entities: &#x41;&#x42;&#x43; represent ABC.";
            var expectedOutput = "Hex entities: ABC represent ABC.";

            var result = TestHelper.TestConvert<HtmlDecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_MixedEntities_ReturnsDecodedString()
        {
            var input = "Mixed: &lt;div&gt;Hello &#x57;&#x6F;&#x72;&#x6C;&#x64; &amp;amp; &#36;100&#33;&lt;/div&gt;";
            var expectedOutput = @"Mixed: <div>Hello World &amp; $100!</div>";

            var result = TestHelper.TestConvert<HtmlDecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_NoEntities_ReturnsOriginalString()
        {
            var input = "This string has no HTML entities.";
            var expectedOutput = input;

            var result = TestHelper.TestConvert<HtmlDecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyString_ReturnsEmptyString()
        {
            var input = "";
            var expectedOutput = "";

            var result = TestHelper.TestConvert<HtmlDecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
