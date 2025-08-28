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
    public class UnicodeEscapingConverterTests
    {
        [Fact]
        public void TestConvert_UnicodeEscaping_ReturnsEscapedText()
        {
            var input = "Hello, 世界!";
            var expectedOutput = @"\u0048\u0065\u006C\u006C\u006F\u002C\u0020\u4E16\u754C\u0021";

            var result = TestHelper.TestConvert<UnicodeEscapingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_UnicodeEscapingWithDifferentPrefixAndDelimiter_ReturnsEscapedText()
        {
            var input = "Hello, 世界!";
            var expectedOutput = @"U+0048,U+0065,U+006C,U+006C,U+006F,U+002C,U+0020,U+4E16,U+754C,U+0021";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(UnicodeEscapingConverter.ParameterKeys.Delimiter, UnicodeEscapingConverter.DelimiterType.Comma),
                new KeyValuePair<string, object>(UnicodeEscapingConverter.ParameterKeys.Prefix, UnicodeEscapingConverter.PrefixType.UPlus),
            };

            var result = TestHelper.TestConvert<UnicodeEscapingConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyString_ReturnsEmptyString()
        {
            var input = "";
            var expectedOutput = "";

            var result = TestHelper.TestConvert<UnicodeEscapingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_SurrogatePairs_ReturnsEscapedText()
        {
            var input = "𠮷野家";
            var expectedOutput = @"%uD842,%uDFB7,%u91CE,%u5BB6";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(UnicodeEscapingConverter.ParameterKeys.Delimiter, UnicodeEscapingConverter.DelimiterType.Comma),
                new KeyValuePair<string, object>(UnicodeEscapingConverter.ParameterKeys.Prefix, UnicodeEscapingConverter.PrefixType.PercentU),
            };

            var result = TestHelper.TestConvert<UnicodeEscapingConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
