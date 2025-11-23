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
    public class CsvConverterTests
    {
        private static readonly string ExampleCsv_NoQuotes =
@"Name,Age,Location,Pocket Money
Alice,30,New York,35220
Bob,25,Los Angeles,1640
Charlie,35,Chicago,900";
        private static readonly string ExampleCsv_DoubleQuotes =
@"""Name"",""Age"",""Location"",""Pocket Money"",""Memo""
""Alice"",""30"",""New York"",""35220"",""""
""Bob"",""25"",""Los Angeles"",""1640"",""This memo contains
LineBreak chars""
""Charlie"",""35"",""Chicago"",""900"",""foobar""";
        private static readonly string ExampleTsv_SingleQuotes =
@"'Name'	'Age'	'Location'	'Pocket Money'	'Memo'
'Alice'	'30'	'New York'	'35220'	''
'Bob'	'25'	'Los Angeles'	'1640'	'This memo contains
LineBreak chars'
'Charlie'	'35'	'Chicago'	'900'	'foobar'";

        [Fact]
        public void TestConvert_SimpleOutputForm_ReturnsTransformedString()
        {
            var input = ExampleCsv_NoQuotes;
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.OutputFormat, @"{0}"),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.Delimiter, ","),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.Enclosure, CsvConverter.EnclosureType.None),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.IgnoreHeader, false),
            };
            var expectedOutput =
@"Name
Alice
Bob
Charlie";

            var result = TestHelper.TestConvert<CsvConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_TsvWithSingleQuotes_ReturnsTransformedString()
        {
            var input = ExampleTsv_SingleQuotes;
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.OutputFormat, @"{3} - {0} ({1} years old)"),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.Delimiter, @"\t"),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.Enclosure, CsvConverter.EnclosureType.SingleQuote),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.IgnoreHeader, false),
            };
            var expectedOutput =
@"Pocket Money - Name (Age years old)
35220 - Alice (30 years old)
1640 - Bob (25 years old)
900 - Charlie (35 years old)";

            var result = TestHelper.TestConvert<CsvConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_CsvWithDoubleQuotes_IgnoreHeader_ReturnsTransformedString()
        {
            var input = ExampleCsv_DoubleQuotes;
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.OutputFormat, @"{0} lives in {2}."),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.Delimiter, ","),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.Enclosure, CsvConverter.EnclosureType.DoubleQuote),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.IgnoreHeader, true),
            };
            var expectedOutput =
@"Alice lives in New York.
Bob lives in Los Angeles.
Charlie lives in Chicago.";

            var result = TestHelper.TestConvert<CsvConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_RegexFormat_ReturnsTransformedString()
        {
            var input = ExampleCsv_NoQuotes;
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.OutputFormat, @"{2/ .+//}"),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.Delimiter, ","),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.Enclosure, CsvConverter.EnclosureType.DoubleQuote),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.IgnoreHeader, true),
            };
            var expectedOutput =
@"New
Los
Chicago";

            var result = TestHelper.TestConvert<CsvConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_NumericFormatExpression_ReturnsTransformedString()
        {
            var input = ExampleCsv_NoQuotes;
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.OutputFormat, @"{3,8:N0}"),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.Delimiter, ","),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.Enclosure, CsvConverter.EnclosureType.None),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.IgnoreHeader, true),
            };
            var expectedOutput =
@"  35,220
   1,640
     900";

            var result = TestHelper.TestConvert<CsvConverter>(input, parameters);
            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyOutputForm_ReturnsInputString()
        {
            var input = ExampleCsv_NoQuotes;
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.OutputFormat, ""),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.Delimiter, ","),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.Enclosure, CsvConverter.EnclosureType.None),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.IgnoreHeader, false),
            };
            var expectedOutput = input;

            var result = TestHelper.TestConvert<CsvConverter>(input, parameters);

            Assert.True(result.Success);
            Assert.Equal(expectedOutput, result.Value);
        }

        [Fact]
        public void TestConvert_EmptyDelimiter_ReturnsError()
        {
            var input = ExampleCsv_NoQuotes;
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.OutputFormat, @"{0}"),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.Delimiter, ""),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.Enclosure, CsvConverter.EnclosureType.None),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.IgnoreHeader, false),
            };

            var result = TestHelper.TestConvert<CsvConverter>(input, parameters);

            Assert.False(result.Success);
            Assert.True(result.Errors.Any());
        }

        [Fact]
        public void TestConvert_MultipleDelimiter_ReturnsError()
        {
            var input = ExampleCsv_NoQuotes;
            var parameters = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.OutputFormat, @"{0}"),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.Delimiter, ",;"),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.Enclosure, CsvConverter.EnclosureType.None),
                new KeyValuePair<string, object>(CsvConverter.ParameterKeys.IgnoreHeader, false),
            };

            var result = TestHelper.TestConvert<CsvConverter>(input, parameters);

            Assert.False(result.Success);
            Assert.True(result.Errors.Any());
        }
    }
}
