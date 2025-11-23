using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("CSV row transformer")]
    public class CsvConverter : PluginBase, IConverter
    {
        public class ParameterKeys
        {
            public const string OutputFormat = "OutputFormat";
            public const string Delimiter = "Delimiter";
            public const string Enclosure = "Enclosure";
            public const string IgnoreHeader = "IgnoreHeader";
        }

        public enum EnclosureType
        {
            None,
            [Display(Name = "Double Quote (\")")]
            DoubleQuote,
            [Display(Name = "Single Quote (')")]
            SingleQuote
        }

        private static readonly Regex PatternColumnExpression = new(@"
(?<=^|[^{])
\{
(?<Index>\d+)
(
    (
        (,(?<Alignment>-?\d+))?
        (:(?<Format>[^}]+))?
    )|
    (
        (?<Delimiter>.)
        (?<Pattern>.*?)
        \k<Delimiter>
        (?<Replacement>.*?)
        \k<Delimiter>
        (?<Flags>[ig]*)
    )?
)
\}",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace
        );

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var outputFormat = parameters[ParameterKeys.OutputFormat].GetValue<string>();
            var delimiter = UnescapeDelimiter(parameters[ParameterKeys.Delimiter].GetValue<string>());
            var enclosure = parameters[ParameterKeys.Enclosure].GetValue<EnclosureType>();
            var ignoreHeader = parameters[ParameterKeys.IgnoreHeader].GetValue<bool>();

            if (delimiter.Length != 1)
            {
                return PluginInvocationResult.OfFailure($"Delimiter must be a single character.");
            }
            if (string.IsNullOrEmpty(outputFormat))
            {
                return PluginInvocationResult.OfSuccess(input);
            }

            var maxColumnIndexOfOutputFormat = GetMaxColumnIndex(outputFormat);
            var rows = ConvertCsvToArray(input, delimiter[0], enclosure);
            var minColumnIndexOfInput = rows.Count == 0 ? 0 : rows.Min(r => r.Length - 1);
            if (minColumnIndexOfInput < maxColumnIndexOfOutputFormat)
            {
                return PluginInvocationResult.OfFailure($"Input CSV does not have enough columns. Required column index: {maxColumnIndexOfOutputFormat}, but the minimum column index in the input is {minColumnIndexOfInput}.");
            }

            var sb = new StringBuilder();
            try
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    if (i == 0 && ignoreHeader)
                    {
                        continue;
                    }

                    var transformedLine = TransformRow(rows[i], outputFormat);
                    sb.AppendLine(transformedLine);
                }
                // Remove the last newline
                sb.Remove(sb.Length - Environment.NewLine.Length, Environment.NewLine.Length);
            }
            catch (FormatException ex)
            {
                return PluginInvocationResult.OfFailure($"Invalid output format: {ex.Message}");
            }

            return PluginInvocationResult.OfSuccess(sb.ToString());
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddTextParameter(ParameterKeys.OutputFormat, ParameterKeys.OutputFormat, "",
@"Specify the output format.
Values in the nth column (0-origin) of the CSV can be referenced using the {n} format.
The following format expressions are supported:
* Numeric value formatting operations supported by the string.Format method
* Replacement using regular expressions ({n/pattern/replacement/flags})");
            configuration.AddTextParameter(ParameterKeys.Delimiter, ParameterKeys.Delimiter, ",");
            configuration.AddListParameter(ParameterKeys.Enclosure, EnclosureType.None);
            configuration.AddFlagParameter(ParameterKeys.IgnoreHeader);
        }

        private string UnescapeDelimiter(string delimiter)
        {
            return delimiter switch
            {
                "\\t" => "\t",
                "\\n" => "\n",
                "\\r" => "\r",
                _ => delimiter,
            };
        }

        private int GetMaxColumnIndex(string outputFormat)
        {
            var maxIndex = -1;
            var matches = PatternColumnExpression.Matches(outputFormat);
            foreach (Match match in matches)
            {
                if (int.TryParse(match.Groups["Index"].Value, out var index))
                {
                    if (index > maxIndex)
                    {
                        maxIndex = index;
                    }
                }
            }
            return maxIndex;
        }

        private List<string[]> ConvertCsvToArray(string input, char delimiter, EnclosureType enclosure)
        {
            if (string.IsNullOrEmpty(input))
            {
                return [];
            }

            var rows = new List<string[]>();
            var currentRow = new List<string>();
            var fieldBuilder = new StringBuilder();

            bool inQuotedField = false;
            char quoteChar = '\0';
            if (enclosure == EnclosureType.DoubleQuote)
            {
                quoteChar = '"';
            }
            else if (enclosure == EnclosureType.SingleQuote)
            {
                quoteChar = '\'';
            }

            int length = input.Length;
            for (int i = 0; i < length; i++)
            {
                char ch = input[i];

                // If currently inside a quoted field
                if (inQuotedField)
                {
                    if (ch == quoteChar)
                    {
                        // RFC 4180: two consecutive quotes inside a quoted field represent an escaped quote
                        if (i + 1 < length && input[i + 1] == quoteChar)
                        {
                            fieldBuilder.Append(quoteChar);
                            i++; // consume escaped quote
                        }
                        else
                        {
                            // closing quote
                            inQuotedField = false;
                        }
                    }
                    else
                    {
                        // inside quoted field, everything (including CR/LF and delimiter) is data
                        fieldBuilder.Append(ch);
                    }

                    continue;
                }

                // Not inside quoted field
                if (enclosure != EnclosureType.None && ch == quoteChar)
                {
                    // begin quoted field
                    inQuotedField = true;
                    continue;
                }

                if (ch == delimiter)
                {
                    currentRow.Add(fieldBuilder.ToString());
                    fieldBuilder.Clear();
                    continue;
                }

                if (ch == '\r' || ch == '\n')
                {
                    // Handle CRLF as single newline
                    if (ch == '\r' && i + 1 < length && input[i + 1] == '\n')
                    {
                        i++; // skip '\n' of CRLF
                    }

                    currentRow.Add(fieldBuilder.ToString());
                    fieldBuilder.Clear();
                    rows.Add(currentRow.ToArray());
                    currentRow.Clear();
                    continue;
                }

                // regular character
                fieldBuilder.Append(ch);
            }

            // Determine if input ended with newline (CR or LF)
            bool endsWithNewline = input.Length > 0 && (input[input.Length - 1] == '\n' || input[input.Length - 1] == '\r');

            // Add the final field/row only when input does not end with a newline.
            if (!endsWithNewline || inQuotedField)
            {
                currentRow.Add(fieldBuilder.ToString());
                rows.Add(currentRow.ToArray());
            }

            return rows;
        }

        private string TransformRow(string[] row, string outputFormat)
        {
            var outputLine = PatternColumnExpression.Replace(outputFormat, match =>
            {
                var index = int.Parse(match.Groups["Index"].Value);
                if (index < 0 || index >= row.Length)
                {
                    return string.Empty;
                }
                var value = row[index];
                // Check for regex replacement
                if (match.Groups["Pattern"].Success && match.Groups["Replacement"].Success)
                {
                    var pattern = match.Groups["Pattern"].Value;
                    var replacement = match.Groups["Replacement"].Value;
                    var flags = match.Groups["Flags"].Value;
                    var options = RegexOptions.None;
                    if (flags.Contains("i"))
                    {
                        options |= RegexOptions.IgnoreCase;
                    }
                    var replaceCount = flags.Contains("g") ? -1 : 1;
                    var regex = new Regex(pattern, options);
                    value = regex.Replace(value, replacement, replaceCount);
                    return value;
                }
                // Check for format and alignment
                var format = match.Groups["Format"].Success ? match.Groups["Format"].Value : null;
                var alignmentStr = match.Groups["Alignment"].Success ? match.Groups["Alignment"].Value : null;
                if (format != null && decimal.TryParse(value, out var decimalValue))
                {
                    value = string.Format($"{{0:{format}}}", decimalValue);
                }
                if (alignmentStr != null && int.TryParse(alignmentStr, out var alignment))
                {
                    value = string.Format($"{{0,{alignment}}}", value);
                }
                return value;
            });
            return outputLine;
        }
    }
}
