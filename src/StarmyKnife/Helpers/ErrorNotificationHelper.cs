using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace StarmyKnife.Helpers
{
    internal static class ErrorNotificationHelper
    {
        internal static void DisplayPluginLoadError(string errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                return;
            }

            try
            {
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher != null)
                {
                    dispatcher.BeginInvoke(
                        new Action(() => MessageBox.Show(errorMessage, "Plugin Load Error", MessageBoxButton.OK, MessageBoxImage.Warning)),
                        DispatcherPriority.ApplicationIdle
                    );
                }
            }
            catch
            {
                // If notification fails, ignore
            }
        }
    }
}
