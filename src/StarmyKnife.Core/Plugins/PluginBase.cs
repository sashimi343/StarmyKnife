using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins
{
    public abstract class PluginBase : IPlugin
    {
        private readonly PluginParameterCollection _parameterSchema;

        public PluginBase()
        {
            var configuration = new PluginParametersConfiguration();
            ConfigureParameters(configuration);
            _parameterSchema = configuration.GetParametersSchema();
        }

        public PluginParameterCollection GetParametersSchema()
        {
            return _parameterSchema.Clone();
        }

        protected abstract void ConfigureParameters(PluginParametersConfiguration configuration);
    }
}
