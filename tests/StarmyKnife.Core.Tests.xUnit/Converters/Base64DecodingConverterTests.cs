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
    public class Base64DecodingConverterTests
    {
        [Fact]
        public void TestConvert_ValidBase64String_ReturnsDecodedString()
        {
            var input = "SGVsbG8gV29ybGQ=";
            var expectedOutput = "Hello World";

            var result = TestHelper.TestConvert<Base64DecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_InvalidBase64String_ReturnsError()
        {
            var input = "InvalidBase64String";

            var result = TestHelper.TestConvert<Base64DecodingConverter>(input);

            Assert.False(result.Success);
            Assert.True(result.Errors.Any());
        }

        [Fact]
        public void TestConvert_EmptyString_ReturnsEmptyString()
        {
            var input = "";
            var expectedOutput = "";

            var result = TestHelper.TestConvert<Base64DecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_NullInput_ReturnsError()
        {
            string input = null;

            var result = TestHelper.TestConvert<Base64DecodingConverter>(input);

            Assert.False(result.Success);
            Assert.True(result.Errors.Any());
        }

        [Fact]
        public void TestConvert_WhitespaceInput_ReturnsEmptyString()
        {
            var input = "   ";
            var expectedOutput = "";

            var result = TestHelper.TestConvert<Base64DecodingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_Base64WithDifferentEncoding_ReturnsCorrectlyDecodedString()
        {
            var input = "44GT44KT44Gr44Gh44Gv";
            var expectedOutput = "こんにちは";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(Base64DecodingConverter.ParameterKeys.CharacterSet, Base64CharacterSet.Standard),
                new KeyValuePair<string, object>(Base64DecodingConverter.ParameterKeys.Encoding, PluginEncoding.UTF8)
            };

            var result = TestHelper.TestConvert<Base64DecodingConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_UrlSafeBase64String_ReturnsCorrectlyDecodedString()
        {
            var input = "w7_jgb7jgZ_jga_DvOOBp-OBmQ";
            var expectedOutput = "ÿまたはüです";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(Base64DecodingConverter.ParameterKeys.CharacterSet, Base64CharacterSet.UrlSafe)
            };

            var result = TestHelper.TestConvert<Base64DecodingConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_FilenameSafeBase64String_ReturnsCorrectlyDecodedString()
        {
            var input = "grGC6oLNk-yXzWRhdGGCxYK3gqmBSA==";
            var expectedOutput = "これは入力dataですか？";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(Base64DecodingConverter.ParameterKeys.CharacterSet, Base64CharacterSet.FilenameSafe),
                new KeyValuePair<string, object>(Base64EncodingConverter.ParameterKeys.Encoding, PluginEncoding.ShiftJIS)
            };

            var result = TestHelper.TestConvert<Base64DecodingConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
