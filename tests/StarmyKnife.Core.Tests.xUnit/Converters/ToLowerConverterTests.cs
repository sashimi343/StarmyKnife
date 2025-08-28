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
    public class ToLowerConverterTests
    {
        [Fact]
        public void TestConvert_UppercaseText_ReturnsLowercaseText()
        {
            var input = "HELLO, WORLD!";
            var expectedOutput = "hello, world!";

            var result = TestHelper.TestConvert<ToLowerConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_MixedCaseText_ReturnsLowercaseText()
        {
            var input = "Hello, World!";
            var expectedOutput = "hello, world!";

            var result = TestHelper.TestConvert<ToLowerConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyString_ReturnsEmptyString()
        {
            var input = string.Empty;
            var expectedOutput = string.Empty;

            var result = TestHelper.TestConvert<ToLowerConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_NonAlphabeticCharacters_ReturnsSameString()
        {
            var input = "12345!@#$% あいうえお";
            var expectedOutput = "12345!@#$% あいうえお";

            var result = TestHelper.TestConvert<ToLowerConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
