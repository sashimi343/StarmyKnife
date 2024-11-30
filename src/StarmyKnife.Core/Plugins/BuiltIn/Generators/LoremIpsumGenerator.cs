using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Generators
{
    [StarmyKnifePlugin("Lorem Ipsum")]
    public class LoremIpsumGenerator : PluginBase, IGenerator
    {
        private class ParameterKeys
        {
            public const string Length = "Length";
            public const string LengthIn = "LengthIn";
        }

        private enum LengthInType
        {
            Paragraph,
            Sentence,
            Word,
        }

        private static readonly string[] Paragraphs = [
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
        ];
        private static readonly string[] Sentences = [
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
            "Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
            "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
            "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
        ];
        private static readonly string[] Words = [
            "Lorem", "ipsum", "dolor", "sit", "amet,", "consectetur", "adipiscing", "elit.",
            "sed", "do", "eiusmod", "tempor", "incididunt", "ut", "labore", "et", "dolore", "magna", "aliqua.",
            "Ut", "enim", "ad", "minim", "veniam,", "quis", "nostrud", "exercitation", "ullamco", "laboris", "nisi", "aliquip", "ex", "ea", "commodo", "consequat.",
            "Duis", "aute", "irure", "in", "reprehenderit", "voluptate", "velit", "esse", "cillum", "fugiat", "nulla", "pariatur.",
            "Excepteur", "sint", "occaecat", "cupidatat", "non", "proident,", "sunt", "culpa", "qui", "officia", "deserunt", "mollit", "anim", "id", "est", "laborum."
        ];

        public string Generate(PluginParameterCollection parameters)
        {
            var length = parameters[ParameterKeys.Length].GetValue<int>();
            var lengthIn = parameters[ParameterKeys.LengthIn].GetValue<LengthInType>();
            var generatedText = new StringBuilder();
            var source = GetGenerationSource(lengthIn);

            for (var i = 0; i < length; i++)
            {
                var index = i % source.Length;
                generatedText.Append(source[index]);
                generatedText.Append(" ");
            }

            // Remove the trailing space
            generatedText.Remove(generatedText.Length - 1, 1);

            return generatedText.ToString();
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddIntegerParameter(ParameterKeys.Length, 3);
            configuration.AddListParameter<LengthInType>(ParameterKeys.LengthIn);
        }

        private string[] GetGenerationSource(LengthInType lengthIn)
        {
            switch (lengthIn)
            {
                case LengthInType.Paragraph:
                    return Paragraphs;
                case LengthInType.Sentence:
                    return Sentences;
                case LengthInType.Word:
                    return Words;
                default:
                    throw new NotImplementedException($"Invaid GenerationType {lengthIn}");
            }
        }
    }
}
