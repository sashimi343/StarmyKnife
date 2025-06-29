using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Core.Plugins.Internal
{
    public class PrettifierToConverterAdapter : IPlugin, IConverter
    {
        private readonly IPrettyValidator _prettyValidator;
        private readonly IPluginMetadata _pluginMetadata;

        internal PrettifierToConverterAdapter(IPrettyValidator prettyValidator)
        {
            _prettyValidator = prettyValidator ?? throw new ArgumentNullException(nameof(prettyValidator));

            if (_prettyValidator.CanPrettify == false)
            {
                throw new ArgumentException("The provided IPrettyValidator does not support prettification.", _prettyValidator.GetType().Name);
            }

            _pluginMetadata = GetPluginMetadata();
        }

        public IPluginMetadata Metadata => _pluginMetadata;

        public PluginInvocationResult Convert(string input, PluginParameterCollection parameters)
        {
            var validationResult = _prettyValidator.Validate(input, parameters);
            if (!validationResult.Success)
            {
                return PluginInvocationResult.OfFailure(validationResult.Errors);
            }

            var prettificationResult = _prettyValidator.Prettify(input, parameters);
            return prettificationResult;
        }

        public PluginParameterCollection GetParametersSchema()
        {
            return _prettyValidator.GetParametersSchema();
        }

        private IPluginMetadata GetPluginMetadata()
        {
            var attributes = _prettyValidator.GetType().GetCustomAttributes(typeof(StarmyKnifePluginAttribute), true);
            if (attributes == null || attributes.Length == 0)
            {
                throw new ArgumentException("The provided IPrettyValidator does not have StarmyKnifePluginAttribute.", _prettyValidator.GetType().Name);
            }
            var starmyKnifePluginAttribute = (StarmyKnifePluginAttribute)attributes[0];

            var metadata = new PluginMetadata()
            {
                Name = "Prettify " + starmyKnifePluginAttribute.Name,
            };

            return metadata;
        }
    }
}
