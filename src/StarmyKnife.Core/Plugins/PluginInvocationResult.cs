using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins
{
    public sealed class PluginInvocationResult
    {
        private readonly bool _success;
        private readonly string _value;
        private readonly string[] _errors;

        private PluginInvocationResult(bool success, string value, IEnumerable<string> errors)
        {
            _success = success;
            _value = value;
            _errors = errors.ToArray();
        }

        public bool Success => _success;
        public string Value => _value;
        public IEnumerable<string> Errors => _errors;

        public static PluginInvocationResult OfSuccess(string value)
        {
            return new PluginInvocationResult(true, value, []);
        }

        public static PluginInvocationResult OfFailure(string error)
        {
            return new PluginInvocationResult(false, "", [error]);
        }

        public static PluginInvocationResult OfFailure(IEnumerable<string> errors)
        {
            return new PluginInvocationResult(false, "", errors.ToArray());
        }
    }
}
