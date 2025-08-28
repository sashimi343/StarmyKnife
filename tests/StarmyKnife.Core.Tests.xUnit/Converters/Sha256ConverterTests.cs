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
    public class Sha256ConverterTests
    {
        [Fact]
        public void TestConvert_SHA256Hash_ReturnsCorrectHash()
        {
            var input = "Hello, World!";
            var expectedOutput = "dffd6021bb2bd5b0af676290809ec3a53191dd81c7f70a4b28688a362182986f";

            var result = TestHelper.TestConvert<Sha256Converter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_SHA256HashWithDifferentEncoding_ReturnsCorrectHash()
        {
            var input = "あいうえお";
            var expectedOutput = "4f23ff3c75d080e0671c7c4512e836ae2c7bc9def2748abad74bc62aa4634c6d";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(Sha256Converter.ParameterKeys.Encoding, PluginEncoding.ShiftJIS)
            };

            var result = TestHelper.TestConvert<Sha256Converter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyString_ReturnsHashValueOfEmptyString()
        {
            var input = "";
            var expectedOutput = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

            var result = TestHelper.TestConvert<Sha256Converter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
