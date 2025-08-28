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
    public class GrepConverterTests
    {
        [Fact]
        public void TestConvert_SimpleTextMatch_ReturnsMatchedLines()
        {
            var input = string.Join(Environment.NewLine, ["apple", "banana", "apricot", "grape", "orange"]);
            var expectedOutput = string.Join(Environment.NewLine, ["apple", "apricot", "grape"]);
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.Pattern, "ap"),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.UseRegex, false),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.IgnoreCase, false),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.InvertMatch, false),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.MatchedPartOnly, false)
            };

            var result = TestHelper.TestConvert<GrepConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_RegexMatchIgnoreCase_ReturnsMatchedLines()
        {
            var input = string.Join(Environment.NewLine, ["Cat", "dog", "caterpillar", "elephant", "Cattle"]);
            var expectedOutput = string.Join(Environment.NewLine, ["Cat", "caterpillar", "Cattle"]);
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.Pattern, "^cat"),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.UseRegex, true),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.IgnoreCase, true),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.InvertMatch, false),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.MatchedPartOnly, false)
            };

            var result = TestHelper.TestConvert<GrepConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_InvertMatch_ReturnsNonMatchedLines()
        {
            var input = string.Join(Environment.NewLine, ["red", "blue", "green", "yellow", "purple"]);
            var expectedOutput = string.Join(Environment.NewLine, ["blue", "yellow"]);
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.Pattern, "r"),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.UseRegex, false),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.IgnoreCase, false),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.InvertMatch, true),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.MatchedPartOnly, false)
            };

            var result = TestHelper.TestConvert<GrepConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_MatchedPartOnly_ReturnsOnlyMatchedParts()
        {
            var input = string.Join(Environment.NewLine, ["Ver.2.1.5", "v1.2", "version: 0.9.5-alpha"]);
            var expectedOutput = string.Join(Environment.NewLine, ["2.1.5", "1.2", "0.9.5"]);
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.Pattern, @"\d+(\.\d+)*"),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.UseRegex, true),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.IgnoreCase, false),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.InvertMatch, false),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.MatchedPartOnly, true)
            };

            var result = TestHelper.TestConvert<GrepConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_NoMatches_ReturnsEmptyString()
        {
            var input = string.Join(Environment.NewLine, ["one", "two", "three"]);
            var expectedOutput = "";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.Pattern, "four"),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.UseRegex, false),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.IgnoreCase, false),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.InvertMatch, false),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.MatchedPartOnly, false)
            };

            var result = TestHelper.TestConvert<GrepConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyInput_ReturnsEmptyString()
        {
            var input = "";
            var expectedOutput = "";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.Pattern, @"any\"),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.UseRegex, false),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.IgnoreCase, false),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.InvertMatch, false),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.MatchedPartOnly, false)
            };

            var result = TestHelper.TestConvert<GrepConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_InvalidRegexPattern_ReturnsFailure()
        {
            var input = string.Join(Environment.NewLine, ["sample text"]);
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.Pattern, @"[unclosed"),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.UseRegex, true),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.IgnoreCase, false),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.InvertMatch, false),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.MatchedPartOnly, false)
            };

            var result = TestHelper.TestConvert<GrepConverter>(input, parameters);

            Assert.False(result.Success);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void TestConvert_EmptyPattern_ReturnsOutputItself()
        {
            var input = string.Join(Environment.NewLine, ["line1", "line2", "line3"]);
            var expectedOutput = input;
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.Pattern, ""),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.UseRegex, false),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.IgnoreCase, false),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.InvertMatch, false),
                new KeyValuePair<string, object>(GrepConverter.ParameterKeys.MatchedPartOnly, false)
            };

            var result = TestHelper.TestConvert<GrepConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
