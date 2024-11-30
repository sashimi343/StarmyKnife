using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml;

namespace StarmyKnife.Core.Plugins.BuiltIn.PrettyValidators
{
    [StarmyKnifePlugin("XML")]
    public class XmlPrettyValidator : PluginBase, IPrettyValidator
    {
        private class ParameterKeys
        {
            public const string Indent = "Indent";
        }

        private enum IndentType
        {
            Tab,
            [Display(Name = "2 Spaces")]
            TwoSpace,
            [Display(Name = "4 Spaces")]
            FourSpace,
            [Display(Name = "8 Spaces")]
            EightSpace,
            None,
        }

        public bool CanPrettify => true;

        public bool CanMinify => true;

        public PluginInvocationResult Minify(string input, PluginParameterCollection parameters)
        {
            var validationResult = ValidateInternal(input, parameters, out XmlDocument xml);
            if (validationResult.Success)
            {
                var minifiedXml = xml.OuterXml;
                return PluginInvocationResult.OfSuccess(minifiedXml);
            }
            else
            {
                return PluginInvocationResult.OfFailure(validationResult.Errors);
            }
        }

        public PluginInvocationResult Prettify(string input, PluginParameterCollection parameters)
        {
            var validationResult = ValidateInternal(input, parameters, out XmlDocument xml);
            if (validationResult.Success)
            {
                using (StringWriter stringWriter = new StringWriter())
                {
                    var xmlWriterSettings = GetXmlWriterSettings(parameters[ParameterKeys.Indent].GetValue<IndentType>());
                    using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSettings))
                    {
                        xml.WriteTo(xmlWriter);
                    }

                    return PluginInvocationResult.OfSuccess(stringWriter.ToString());
                }
            }
            else
            {
                return PluginInvocationResult.OfFailure(validationResult.Errors);
            }
        }

        public ValidationResult Validate(string input, PluginParameterCollection parameters)
        {
            return ValidateInternal(input, parameters, out XmlDocument _);
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            configuration.AddListParameter<IndentType>(ParameterKeys.Indent);
        }

        private ValidationResult ValidateInternal(string input, PluginParameterCollection parameters, out XmlDocument xml)
        {
            try
            {
                xml = new XmlDocument();
                xml.LoadXml(input);

                return ValidationResult.OfSuccess();
            }
            catch (XmlException e)
            {
                xml = null;
                return ValidationResult.OfFailure($"{e.Message} at Line = {e.LineNumber}, Position = {e.LinePosition}");
            }
            catch (Exception e)
            {
                xml = null;
                return ValidationResult.OfFailure(e.Message);
            }
        }

        private XmlWriterSettings GetXmlWriterSettings(IndentType indentType)
        {
            var xmlWriterSettings = new XmlWriterSettings
            {
                NewLineChars = Environment.NewLine,
                NewLineHandling = NewLineHandling.Replace,
            };

            if (indentType == IndentType.None)
            {
                xmlWriterSettings.Indent = false;
            }
            else
            {
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.IndentChars = GetIndentChars(indentType);
            }

            return xmlWriterSettings;
        }

        private string GetIndentChars(IndentType indentType)
        {
            switch (indentType)
            {
                case IndentType.None:
                    return "";
                case IndentType.Tab:
                    return "\t";
                case IndentType.TwoSpace:
                    return "  ";
                case IndentType.FourSpace:
                    return "    ";
                case IndentType.EightSpace:
                    return "        ";
                default:
                    throw new ArgumentException($"Invalid IndentType {indentType}");
            }
        }
    }
}
