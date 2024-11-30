using System;
using System.Collections.Generic;
using System.Text;

namespace StarmyKnife.Core.Plugins
{
    public sealed class ValidationResult
    {
        private readonly bool _success;
        private readonly string[] _errors;

        private ValidationResult(bool success, IEnumerable<string> errors)
        {
            _success = success;
            _errors = errors.ToArray();
        }

        public bool Success => _success;

        public IEnumerable<string> Errors => _errors;

        public static ValidationResult OfSuccess()
        {
            return new ValidationResult(true, []);
        }

        public static ValidationResult OfFailure(string error)
        {
            return new ValidationResult(false, [error]);
        }

        public static ValidationResult OfFailure(IEnumerable<string> errors)
        {
            return new ValidationResult(false, errors);
        }
    }
}
