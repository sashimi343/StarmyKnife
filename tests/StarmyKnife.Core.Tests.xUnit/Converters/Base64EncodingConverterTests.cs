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
    public class Base64EncodingConverterTests
    {
        [Fact]
        public void TestConvert_ValidString_ReturnsBase64EncodedString()
        {
            var input = "Hello World";
            var expectedOutput = "SGVsbG8gV29ybGQ=";

            var result = TestHelper.TestConvert<Base64EncodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyString_ReturnsEmptyString()
        {
            var input = "";
            var expectedOutput = "";

            var result = TestHelper.TestConvert<Base64EncodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_NullInput_ReturnsError()
        {
            string input = null;

            var result = TestHelper.TestConvert<Base64EncodingConverter>(input);

            Assert.False(result.Success);
            Assert.True(result.Errors.Any());
        }

        [Fact]
        public void TestConvert_DifferentInputEncodings_ReturnsCorrectBase64EncodedString()
        {
            var input = "これは変換テストの入力文字列dayo";
            var expectedOutput = "44GT44KM44Gv5aSJ5o+b44OG44K544OI44Gu5YWl5Yqb5paH5a2X5YiXZGF5bw==";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(Base64EncodingConverter.ParameterKeys.Encoding, PluginEncoding.UTF8)
            };

            var result = TestHelper.TestConvert<Base64EncodingConverter>(input, parameters);
            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_UrlSafeBase64Encoding_ReturnsUrlSafeBase64String()
        {
            var input = "ÿまたはüです";
            var expectedOutput = "w7_jgb7jgZ_jga_DvOOBp-OBmQ";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(Base64EncodingConverter.ParameterKeys.CharacterSet, Base64CharacterSet.UrlSafe)
            };

            var result = TestHelper.TestConvert<Base64EncodingConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_FilenameSafeBase64Encoding_ReturnsFilenameSafeBase64String()
        {
            var input = "これは入力dataですか？";
            var expectedOutput = "grGC6oLNk-yXzWRhdGGCxYK3gqmBSA==";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(Base64EncodingConverter.ParameterKeys.CharacterSet, Base64CharacterSet.FilenameSafe),
                new KeyValuePair<string, object>(Base64EncodingConverter.ParameterKeys.Encoding, PluginEncoding.ShiftJIS)
            };

            var result = TestHelper.TestConvert<Base64EncodingConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
