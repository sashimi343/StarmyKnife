using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("Change case")]
    public class CaseConverter : PluginBase, IConverter
    {
        public class ParameterKeys
        {
            public const string CaseType = "CaseType";
        }

        public enum CaseType
        {
            [Display(Name = "PascalCase")]
            PascalCase,
            [Display(Name = "camelCase")]
            CamelCase,
            [Display(Name = "snake_case")]
            SnakeCase,
            [Display(Name = "kebab-case")]
            KebabCase,
            [Display(Name = "UPPER_SNAKE_CASE")]
            UpperSnakeCase,
        }

        private static readonly Regex PascalCamelNormalizePattern = new(@"([a-z0-9])([A-Z])", RegexOptions.Compiled);
        private static readonly string PascalCamelNormalizeReplacement = "$1 $2";

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var caseType = parameters[ParameterKeys.CaseType].GetValue<CaseType>();
            var words = SplitWords(input);

            string result;
            switch (caseType)
            {
                case CaseType.PascalCase:
                    result = ToPascalCase(words);
                    break;
                case CaseType.CamelCase:
                    result = ToCamelCase(words);
                    break;
                case CaseType.SnakeCase:
                    result = ToSnakeCase(words);
                    break;
                case CaseType.KebabCase:
                    result = ToKebabCase(words);
                    break;
                case CaseType.UpperSnakeCase:
                    result = ToUpperSnakeCase(words);
                    break;
                default:
                    return PluginInvocationResult.OfFailure($"Unsupported case type: {caseType}");
            }

            return PluginInvocationResult.OfSuccess(result);
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddListParameter<CaseType>(ParameterKeys.CaseType);
        }

        private List<string> SplitWords(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new List<string>();
            }

            // Normalize PascalCase and camelCase to space-separated
            var normalized = PascalCamelNormalizePattern.Replace(input, PascalCamelNormalizeReplacement);

            // Replace "_"(snake_case) and "-"(kebab-case) to space
            normalized = normalized.Replace('_', ' ').Replace('-', ' ');

            return normalized.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.ToLower())
                .ToList();
        }

        private string ToPascalCase(List<string> words)
        {
            var result = string.Concat(words.Select(w => char.ToUpper(w[0]) + w.Substring(1)));
            return result;
        }

        private string ToCamelCase(List<string> words)
        {
            if (words.Count == 0)
            {
                return string.Empty;
            }

            var firstWord = words[0];
            var otherWords = words.Skip(1).Select(w => char.ToUpper(w[0]) + w.Substring(1));
            var result = firstWord + string.Concat(otherWords);
            return result;
        }

        private string ToSnakeCase(List<string> words)
        {
            var result = string.Join("_", words);
            return result;
        }

        private string ToKebabCase(List<string> words)
        {
            var result = string.Join("-", words);
            return result;
        }

        private string ToUpperSnakeCase(List<string> words)
        {
            var result = string.Join("_", words).ToUpper();
            return result;
        }
    }
}
