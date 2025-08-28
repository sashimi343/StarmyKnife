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
    public class Sha1ConverterTests
    {
        [Fact]
        public void TestConvert_SHA1Hash_ReturnsCorrectHash()
        {
            var input = "Hello, World!";
            var expectedOutput = "0a0a9f2a6772942557ab5355d76af442f8f65e01";

            var result = TestHelper.TestConvert<Sha1Converter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_SHA1HashWithDifferentEncoding_ReturnsCorrectHash()
        {
            var input = "あいうえお";
            var expectedOutput = "bc2bdc1b335615585f7ef66f09047e3bfcfcd32a";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(Sha1Converter.ParameterKeys.Encoding, PluginEncoding.UTF16LE)
            };

            var result = TestHelper.TestConvert<Sha1Converter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyString_ReturnsHashValueOfEmptyString()
        {
            var input = "";
            var expectedOutput = "da39a3ee5e6b4b0d3255bfef95601890afd80709";

            var result = TestHelper.TestConvert<Sha1Converter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
