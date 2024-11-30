using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Generators
{
    [StarmyKnifePlugin("Random Number")]
    public class RandomNumberGenerator : PluginBase, IGenerator
    {
        private class ParameterKeys
        {
            public const string OutputType = "OutputType";
            public const string MinValue = "MinValue";
            public const string MaxValue = "MaxValue";
        }

        private enum OutputType
        {
            Integer,
            RealNumber,
        }

        public string Generate(PluginParameterCollection parameters)
        {
            var outputType = parameters[ParameterKeys.OutputType].GetValue<OutputType>();
            var minValue = parameters[ParameterKeys.MinValue].GetValue<decimal>();
            var maxValue = parameters[ParameterKeys.MaxValue].GetValue<decimal>();
            var random = new Random();

            switch (outputType)
            {
                case OutputType.Integer:
                    var randomInt = random.Next((int)minValue, (int)maxValue);
                    return randomInt.ToString();
                case OutputType.RealNumber:
                    var range = (double)(maxValue - minValue);
                    var randomDouble = random.NextDouble() * range + (double)minValue;
                    return randomDouble.ToString();
                default:
                    return "";
            }
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddListParameter<OutputType>(ParameterKeys.OutputType, OutputType.Integer);
            configuration.AddDecimalParameter(ParameterKeys.MinValue, 0);
            configuration.AddDecimalParameter(ParameterKeys.MaxValue, 100);
        }
    }
}
