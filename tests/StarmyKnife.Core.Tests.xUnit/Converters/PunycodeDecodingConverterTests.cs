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
    public class PunycodeDecodingConverterTests
    {
        [Fact]
        public void TestConvert_ValidPunycode_ReturnsDecodedString()
        {
            var input = "xn--fsq";
            var expectedOutput = "例";

            var result = TestHelper.TestConvert<PunycodeDecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_NonPunycode_ReturnsOriginalString()
        {
            var input = "non-punycode";
            var expectedOutput = "non-punycode";

            var result = TestHelper.TestConvert<PunycodeDecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyString_ReturnsEmptyString()
        {
            var input = "";
            var expectedOutput = "";

            var result = TestHelper.TestConvert<PunycodeDecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
