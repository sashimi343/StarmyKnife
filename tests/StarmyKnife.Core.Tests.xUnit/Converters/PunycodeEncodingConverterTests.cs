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
    public class PunycodeEncodingConverterTests
    {
        [Fact]
        public void TestConvert_ValidString_ReturnsPunycode()
        {
            var input = "例";
            var expectedOutput = "xn--fsq";
            var result = TestHelper.TestConvert<PunycodeEncodingConverter>(input);
            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_AsciiString_ReturnsSameString()
        {
            var input = "ascii";
            var expectedOutput = "ascii";
            var result = TestHelper.TestConvert<PunycodeEncodingConverter>(input);
            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyString_ReturnsEmptyString()
        {
            var input = "";
            var expectedOutput = "";
            var result = TestHelper.TestConvert<PunycodeEncodingConverter>(input);
            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
