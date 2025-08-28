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
    public class ReplaceTextConverterTests
    {
        [Fact]
        public void TestConvert_LiteralPatternInput_ReturnsReplacedText()
        {
            var input = "Hello World! Hello Universe!";
            var expectedOutput = "Hi World! Hi Universe!";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Pattern, "Hello"),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Replacement, "Hi"),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.UseRegex, false),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.IgnoreCase, false),
            };

            var result = TestHelper.TestConvert<ReplaceTextConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_RegexPatternInput_ReturnsReplacedText()
        {
            var input = "cat, cot, cut, cit";
            var expectedOutput = "dog, dog, cut, cit";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Pattern, "c[ao]t"),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Replacement, "dog"),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.UseRegex, true),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.IgnoreCase, false),
            };

            var result = TestHelper.TestConvert<ReplaceTextConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_IgnoreCaseOption_WorksCorrectly()
        {
            var input = "Hello hello HeLLo";
            var expectedOutput = "Hi Hi Hi";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Pattern, "hello"),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Replacement, "Hi"),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.UseRegex, false),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.IgnoreCase, true),
            };

            var result = TestHelper.TestConvert<ReplaceTextConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_ReplacementWithEscapeSequences_WorksCorrectly()
        {
            var input = "Line1\nLine2\nLine3";
            var expectedOutput = "Line1\r\nLine2\r\nLine3";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Pattern, "\n"),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Replacement, "\r\n"),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.UseRegex, false),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.IgnoreCase, false),
            };

            var result = TestHelper.TestConvert<ReplaceTextConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyPattern_ReturnsOriginalText()
        {
            var input = "No changes here.";
            var expectedOutput = "No changes here.";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Pattern, ""),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Replacement, "Anything"),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.UseRegex, false),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.IgnoreCase, false),
            };

            var result = TestHelper.TestConvert<ReplaceTextConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_NoMatches_ReturnsOriginalText()
        {
            var input = "This text remains unchanged.";
            var expectedOutput = "This text remains unchanged.";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Pattern, "xyz"),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Replacement, "abc"),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.UseRegex, false),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.IgnoreCase, false),
            };

            var result = TestHelper.TestConvert<ReplaceTextConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_InvalidRegexPattern_ReturnsError()
        {
            var input = "Some text";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Pattern, "[unclosed"),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Replacement, "replacement"),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.UseRegex, true),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.IgnoreCase, false),
            };

            var result = TestHelper.TestConvert<ReplaceTextConverter>(input, parameters);

            Assert.False(result.Success);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void TestConvert_RegexSpecialReplacement_WorksCorrectly()
        {
            var input = "abc123def";
            var expectedOutput = "abc-123-def";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Pattern, @"(\d+)"),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Replacement, "-$1-"),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.UseRegex, true),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.IgnoreCase, false),
            };

            var result = TestHelper.TestConvert<ReplaceTextConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_MultipleOccurrences_ReplacesAll()
        {
            var input = "foo foo foo";
            var expectedOutput = "bar bar bar";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Pattern, "foo"),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Replacement, "bar"),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.UseRegex, false),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.IgnoreCase, false),
            };

            var result = TestHelper.TestConvert<ReplaceTextConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyInput_ReturnsEmpty()
        {
            var input = "";
            var expectedOutput = "";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Pattern, "anything"),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.Replacement, "something"),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.UseRegex, false),
                new KeyValuePair<string, object>(ReplaceTextConverter.ParameterKeys.IgnoreCase, false),
            };

            var result = TestHelper.TestConvert<ReplaceTextConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
