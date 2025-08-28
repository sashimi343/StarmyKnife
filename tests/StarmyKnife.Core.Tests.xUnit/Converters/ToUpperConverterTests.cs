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
    public class ToUpperConverterTests
    {
        [Fact]
        public void TestConvert_LowercaseText_ReturnsUppercaseText()
        {
            var input = "hello, world!";
            var expectedOutput = "HELLO, WORLD!";

            var result = TestHelper.TestConvert<ToUpperConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_MixedCaseText_ReturnsUppercaseText()
        {
            var input = "Hello, World!";
            var expectedOutput = "HELLO, WORLD!";

            var result = TestHelper.TestConvert<ToUpperConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyString_ReturnsEmptyString()
        {
            var input = string.Empty;
            var expectedOutput = string.Empty;

            var result = TestHelper.TestConvert<ToUpperConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_NonAlphabeticCharacters_ReturnsSameString()
        {
            var input = "12345!@#$% あいうえお";
            var expectedOutput = "12345!@#$% あいうえお";

            var result = TestHelper.TestConvert<ToUpperConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
