using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace CSVAssistent.Services.ErrorLog
{
    public class UserNotificationService : IUserNotificationService
    {
        public void ShowInfo(string message, string title = "Info") =>
            System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);

        public void ShowWarning(string message, string title = "Warnung") =>
            System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);

        public void ShowError(string message, string title = "Fehler") =>
            System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
