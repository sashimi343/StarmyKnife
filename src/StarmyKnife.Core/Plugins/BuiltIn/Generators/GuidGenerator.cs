using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins.BuiltIn.Generators
{
    [StarmyKnifePlugin("GUID")]
    public class GuidGenerator : PluginBase, IGenerator
    {
        public string Generate(PluginParameterCollection parameters)
        {
            return Guid.NewGuid().ToString();
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
        }
    }
}
