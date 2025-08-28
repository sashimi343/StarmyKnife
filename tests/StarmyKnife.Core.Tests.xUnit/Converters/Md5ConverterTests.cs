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
    public class Md5ConverterTests
    {
        [Fact]
        public void TestConvert_MD5Hash_ReturnsCorrectHash()
        {
            var input = "Hello, World!";
            var expectedOutput = "65a8e27d8879283831b664bd8b7f0ad4";

            var result = TestHelper.TestConvert<Md5Converter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_MD5HashWithDifferentEncoding_ReturnsCorrectHash()
        {
            var input = "あいうえお";
            var expectedOutput = "a7a6dd0ac471eded94c40797a4024332";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(Md5Converter.ParameterKeys.Encoding, PluginEncoding.UTF16BE.Encoding)
            };

            var result = TestHelper.TestConvert<Md5Converter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyString_ReturnsHashValueOfEmptyString()
        {
            var input = "";
            var expectedOutput = "d41d8cd98f00b204e9800998ecf8427e";

            var result = TestHelper.TestConvert<Md5Converter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
