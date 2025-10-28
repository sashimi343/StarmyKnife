using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("Swap endianness")]
    public class SwapEndiannessConverter : PluginBase, IConverter
    {
        public class ParameterKeys
        {
            public const string BytesPerWord = "BytesPerWord";
        }

        private static readonly Regex ValidInputPattern = new(@"^[0-9a-fA-F\s]*$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex RemoveSpacePattern = new(@"\s+", RegexOptions.Compiled | RegexOptions.Singleline);

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            if (!ValidInputPattern.IsMatch(input))
            {
                return PluginInvocationResult.OfFailure("Input contains invalid characters. Only hexadecimal digits and whitespace are allowed.");
            }

            var bytesPerWord = parameters[ParameterKeys.BytesPerWord].GetValue<int>();
            var inputHexString = RemoveSpacePattern.Replace(input, "");

            if (inputHexString.Length % (bytesPerWord * 2) != 0)
            {
                return PluginInvocationResult.OfFailure($"Input length is not a multiple of {bytesPerWord} bytes.");
            }

            var output = new StringBuilder();
            for (int i = 0; i < inputHexString.Length; i += bytesPerWord * 2)
            {
                var word = inputHexString.Substring(i, bytesPerWord * 2);
                for (int j = bytesPerWord - 1; j >= 0; j--)
                {
                    output.Append(word.Substring(j * 2, 2));
                }
            }

            return PluginInvocationResult.OfSuccess(output.ToString());
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddIntegerParameter(ParameterKeys.BytesPerWord, 4);
        }
    }
}
