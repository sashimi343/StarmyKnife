using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StarmyKnife.PluginInterfaces;

namespace StarmyKnife.Core.Tests.xUnit
{
    internal static class TestHelper
    {
        internal static PluginInvocationResult TestConvert<T>(string input, params KeyValuePair<string, object>[] parameters) where T : IConverter, new()
        {
            var parameterCollection = CreateParameterCollection<T>(parameters);
            var converter = new T();
            var result = converter.Convert(input, parameterCollection);

            return result;
        }

        internal static string TestGenerate<T>(params KeyValuePair<string, object>[] parameters) where T : IGenerator, new()
        {
            var parameterCollection = CreateParameterCollection<T>(parameters);
            var generator = new T();
            var result = generator.Generate(parameterCollection);

            return result;
        }

        internal static ValidationResult TestValidate<T>(string input, params KeyValuePair<string, object>[] parameters) where T : IPrettyValidator, new()
        {
            var parameterCollection = CreateParameterCollection<T>(parameters);
            var validator = new T();
            var result = validator.Validate(input, parameterCollection);

            return result;
        }

        internal static PluginInvocationResult TestPrettify<T>(string input, params KeyValuePair<string, object>[] parameters) where T : IPrettyValidator, new()
        {
            var parameterCollection = CreateParameterCollection<T>(parameters);
            var converter = new T();
            var result = converter.Prettify(input, parameterCollection);

            return result;
        }

        internal static PluginInvocationResult TestMinify<T>(string input, params KeyValuePair<string, object>[] parameters) where T : IPrettyValidator, new()
        {
            var parameterCollection = CreateParameterCollection<T>(parameters);
            var converter = new T();
            var result = converter.Minify(input, parameterCollection);

            return result;
        }

        private static PluginParameterCollection CreateParameterCollection<T>(KeyValuePair<string, object>[] paramItems) where T : IPlugin, new()
        {
            var plugin = new T();
            var parameters = plugin.GetParametersSchema();

            foreach (var paramItem in paramItems)
            {
                if (parameters.ContainsKey(paramItem.Key))
                {
                    if (paramItem.Value is PluginEncoding pluginEncoding)
                    {
                        parameters[paramItem.Key].SetValue(pluginEncoding.Encoding);
                    }
                    else
                    {
                        parameters[paramItem.Key].SetValue(paramItem.Value);
                    }
                }
                else
                {
                    throw new KeyNotFoundException($"Parameter '{paramItem.Key}' not found in plugin schema.");
                }
            }

            return parameters;
        }
    }
}
