using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Helpers
{
    public static class ErrorsContainerExtensions
    {
        public static void SetErrorsIfChanged(this ErrorsContainer<string> errorsContainer, string propertyName, string error)
        {
            SetErrorsIfChanged(errorsContainer, propertyName, new string[] {error});
        }

        public static void SetErrorsIfChanged(this ErrorsContainer<string> errorsContainer, string propertyName, string format, params object[] args)
        {
            var error = string.Format(format, args);
            SetErrorsIfChanged(errorsContainer, propertyName, error);
        }

        public static void SetErrorsIfChanged(this ErrorsContainer<string> errorsContainer, string propertyName, IEnumerable<string> errors)
        {
            var currentErrors = errorsContainer.GetErrors(propertyName);

            if (currentErrors == null || !currentErrors.SequenceEqual(errors))
            {
                errorsContainer.SetErrors(propertyName, errors);
            }
        }

        public static void SetErrorsFromException(this ErrorsContainer<string> errorsContainer, string propertyName, Exception exception)
        {
            SetErrorsFromException(errorsContainer, propertyName, Properties.Resources.Common_UnexpectedErrorHasBeenOccurred, exception);
        }

        public static void SetErrorsFromException(this ErrorsContainer<string> errorsContainer, string propertyName, string format, Exception exception)
        {
            var errorMessage = string.Format(format, exception.Message);
            var errors = new[] { errorMessage };
            SetErrorsIfChanged(errorsContainer, propertyName, errors);
        }
    }
}
