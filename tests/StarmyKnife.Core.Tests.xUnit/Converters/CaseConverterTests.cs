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
    public class CaseConverterTests
    {
        [Fact]
        public void TestConvert_PascalCaseText_CanConvertToSnakeCase()
        {
            var input = "ThisIsPascalCase";
            var expectedOutput = "this_is_pascal_case";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(CaseConverter.ParameterKeys.CaseType, CaseConverter.CaseType.SnakeCase),
            };

            var result = TestHelper.TestConvert<CaseConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_CamelCaseText_CanConvertToSnakeCase()
        {
            var input = "thisIsCamelCase";
            var expectedOutput = "this_is_camel_case";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(CaseConverter.ParameterKeys.CaseType, CaseConverter.CaseType.SnakeCase),
            };

            var result = TestHelper.TestConvert<CaseConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_SnakeCaseText_CanConvertToCamelCase()
        {
            var input = "this_is_snake_case";
            var expectedOutput = "thisIsSnakeCase";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(CaseConverter.ParameterKeys.CaseType, CaseConverter.CaseType.CamelCase),
            };

            var result = TestHelper.TestConvert<CaseConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_KebabCaseText_CanConvertToPascalCase()
        {
            var input = "this-is-kebab-case";
            var expectedOutput = "ThisIsKebabCase";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(CaseConverter.ParameterKeys.CaseType, CaseConverter.CaseType.PascalCase),
            };

            var result = TestHelper.TestConvert<CaseConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_UpperSnakeCaseText_CanConvertToKebabCase()
        {
            var input = "THIS_IS_UPPER_SNAKE_CASE";
            var expectedOutput = "this-is-upper-snake-case";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(CaseConverter.ParameterKeys.CaseType, CaseConverter.CaseType.KebabCase),
            };

            var result = TestHelper.TestConvert<CaseConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyString_ReturnsEmptyString()
        {
            var input = string.Empty;
            var expectedOutput = string.Empty;
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(CaseConverter.ParameterKeys.CaseType, CaseConverter.CaseType.SnakeCase),
            };

            var result = TestHelper.TestConvert<CaseConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_SingleWord_ReturnsSameWordInTargetCase()
        {
            var input = "word";
            var expectedOutput = "WORD";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(CaseConverter.ParameterKeys.CaseType, CaseConverter.CaseType.UpperSnakeCase),
            };
            var result = TestHelper.TestConvert<CaseConverter>(input, parameters);
            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_TextWithNumbers_CanConvertCorrectly()
        {
            var input = "ThisIsTest123Text";
            var expectedOutput = "this_is_test123_text";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(CaseConverter.ParameterKeys.CaseType, CaseConverter.CaseType.SnakeCase),
            };

            var result = TestHelper.TestConvert<CaseConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_TextWithMixedDelimiters_CanConvertCorrectly()
        {
            var input = "This-is_mixedDelimiters";
            var expectedOutput = "THIS_IS_MIXED_DELIMITERS";
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(CaseConverter.ParameterKeys.CaseType, CaseConverter.CaseType.UpperSnakeCase),
            };

            var result = TestHelper.TestConvert<CaseConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }
    }
}
