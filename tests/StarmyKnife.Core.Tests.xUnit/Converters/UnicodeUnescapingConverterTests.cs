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
    public class UnicodeUnescapingConverterTests
    {
        [Fact]
        public void TestConvert_BackShashUEscapedText_ReturnsUnescapedText()
        {
            var input = @"\u0048\u0065\u006C\u006C\u006F\u002C\u0020\u4E16\u754C\u0021";
            var expectedOutput = "Hello, 世界!";

            var result = TestHelper.TestConvert<UnicodeUnescapingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_UPlusEscapedText_ReturnsUnescapedText()
        {
            var input = @"U+0048,U+0065,U+006C,U+006C,U+006F,U+002C,U+0020,U+4E16,U+754C,U+0021";
            var expectedOutput = "Hello, 世界!";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(UnicodeUnescapingConverter.ParameterKeys.Delimiter, UnicodeUnescapingConverter.DelimiterType.Comma),
                new KeyValuePair<string, object>(UnicodeUnescapingConverter.ParameterKeys.Prefix, UnicodeUnescapingConverter.PrefixType.UPlus),
            };

            var result = TestHelper.TestConvert<UnicodeUnescapingConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_PercentUEscapedText_ReturnsUnescapedText()
        {
            var input = string.Join(Environment.NewLine, ["%uD842", "%uDFB7", "%u91CE", "%u5BB6"]);
            var expectedOutput = "𠮷野家";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(UnicodeUnescapingConverter.ParameterKeys.Delimiter, UnicodeUnescapingConverter.DelimiterType.NewLine),
                new KeyValuePair<string, object>(UnicodeUnescapingConverter.ParameterKeys.Prefix, UnicodeUnescapingConverter.PrefixType.PercentU),
            };

            var result = TestHelper.TestConvert<UnicodeUnescapingConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyString_ReturnsEmptyString()
        {
            var input = "";
            var expectedOutput = "";

            var result = TestHelper.TestConvert<UnicodeUnescapingConverter>(input);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_InvalidEscapedText_ReturnsError()
        {
            var input = @"\u004X\u0065\u006C\u006C\u006F";

            var result = TestHelper.TestConvert<UnicodeUnescapingConverter>(input);

            Assert.False(result.Success);
            Assert.NotEmpty(result.Errors);
        }
    }
}
