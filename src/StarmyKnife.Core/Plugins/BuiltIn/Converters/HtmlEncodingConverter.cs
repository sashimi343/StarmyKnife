using StarmyKnife.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;

namespace StarmyKnife.Core.Plugins.BuiltIn.Converters
{
    [StarmyKnifePlugin("HTML Encode")]
    public class HtmlEncodingConverter : PluginBase, IConverter
    {
        private class ParameterKeys
        {
            public const string ConvertAllCharacters = "ConvertAllCharacters";
            public const string EncodingMode = "EncodingMode";
        }

        private enum EncodingMode
        {
            ToNamedEntities,
            ToNumericEntities,
            ToHexEntities,
        }

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var convertAllCharacters = parameters[ParameterKeys.ConvertAllCharacters].GetValue<bool>();
            var encodingMode = parameters[ParameterKeys.EncodingMode].GetValue<EncodingMode>();

            if (convertAllCharacters && encodingMode == EncodingMode.ToNamedEntities)
            {
                return PluginInvocationResult.OfFailure("Cannot convert all characters to named entities");
            }

            var output = encodingMode switch
            {
                EncodingMode.ToNamedEntities => HtmlEncodingHelper.ToNamedEntities(input),
                EncodingMode.ToNumericEntities => HtmlEncodingHelper.ToNumericEntities(input, convertAllCharacters),
                EncodingMode.ToHexEntities => HtmlEncodingHelper.ToHexEntities(input, convertAllCharacters),
                _ => null,
            };

            if (output == null)
            {
                return PluginInvocationResult.OfFailure("Invalid encoding mode");
            }
            else
            {
                return PluginInvocationResult.OfSuccess(output);
            }
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddFlagParameter(ParameterKeys.ConvertAllCharacters, false);
            configuration.AddListParameter<EncodingMode>(ParameterKeys.EncodingMode, EncodingMode.ToNamedEntities);
        }
    }
}
