using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Plugins.BuiltIn.Generators
{
    [StarmyKnifePlugin("Random Text")]
    public class RandomTextGenerator : PluginBase, IGenerator
    {
        private class ParameterKeys
        {
            public const string Length = "Length";
            public const string CharCategory_UpperCase = "UpperCase";
            public const string CharCategory_LowerCase = "LowerCase";
            public const string CharCategory_Digits = "Digits";
            public const string CharCategory_Minus = "Minus";
            public const string CharCategory_Underscore = "Underscore";
            public const string CharCategory_Brackets = "Brackets";
            public const string CharCategory_Space = "Space";
            public const string CharCategory_Special = "Special";
            public const string CustomCharset = "CustomCharset";
        }

        private static readonly char[] UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static readonly char[] LowercaseChars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        private static readonly char[] DigitsChars = "0123456789".ToCharArray();
        private static readonly char[] Minus = "-".ToCharArray();
        private static readonly char[] Underscore = "_".ToCharArray();
        private static readonly char[] Brackets = "()[]{}<>".ToCharArray();
        private static readonly char[] Space = " ".ToCharArray();
        private static readonly char[] Special = "!@#$%^&+=*;:,.|?".ToCharArray();

        public string Generate(PluginParameterCollection parameters)
        {
            var length = parameters[ParameterKeys.Length].GetValue<int>();
            var charset = GetCharset(parameters);

            var random = new Random();
            var result = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                var index = random.Next(charset.Count);
                result.Append(charset[index]);
            }

            return result.ToString();
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddIntegerParameter(ParameterKeys.Length, 8);
            configuration.AddFlagParameter(ParameterKeys.CharCategory_UpperCase, true);
            configuration.AddFlagParameter(ParameterKeys.CharCategory_LowerCase, true);
            configuration.AddFlagParameter(ParameterKeys.CharCategory_Digits, true);
            configuration.AddFlagParameter(ParameterKeys.CharCategory_Minus, false);
            configuration.AddFlagParameter(ParameterKeys.CharCategory_Underscore, false);
            configuration.AddFlagParameter(ParameterKeys.CharCategory_Brackets, false);
            configuration.AddFlagParameter(ParameterKeys.CharCategory_Space, false);
            configuration.AddFlagParameter(ParameterKeys.CharCategory_Special, false);
            configuration.AddTextParameter(ParameterKeys.CustomCharset);
        }

        private List<char> GetCharset(PluginParameterCollection parameters)
        {
            var charCategory_UpperCase = parameters[ParameterKeys.CharCategory_UpperCase].GetValue<bool>();
            var charCategory_LowerCase = parameters[ParameterKeys.CharCategory_LowerCase].GetValue<bool>();
            var charCategory_Digits = parameters[ParameterKeys.CharCategory_Digits].GetValue<bool>();
            var charCategory_Minus = parameters[ParameterKeys.CharCategory_Minus].GetValue<bool>();
            var charCategory_Underscore = parameters[ParameterKeys.CharCategory_Underscore].GetValue<bool>();
            var charCategory_Brackets = parameters[ParameterKeys.CharCategory_Brackets].GetValue<bool>();
            var charCategory_Space = parameters[ParameterKeys.CharCategory_Space].GetValue<bool>();
            var charCategory_Special = parameters[ParameterKeys.CharCategory_Special].GetValue<bool>();
            var customCharset = parameters[ParameterKeys.CustomCharset].GetValue<string>();

            var charset = new List<char>();

            if (charCategory_UpperCase) charset.AddRange(UppercaseChars);
            if (charCategory_LowerCase) charset.AddRange(LowercaseChars);
            if (charCategory_Digits) charset.AddRange(DigitsChars);
            if (charCategory_Minus) charset.AddRange(Minus);
            if (charCategory_Underscore) charset.AddRange(Underscore);
            if (charCategory_Brackets) charset.AddRange(Brackets);
            if (charCategory_Space) charset.AddRange(Space);
            if (charCategory_Special) charset.AddRange(Special);

            if (!string.IsNullOrEmpty(customCharset))
            {
                charset.AddRange(customCharset.ToCharArray().Where(c => !charset.Contains(c)));
            }

            return charset;

        }
    }
}
