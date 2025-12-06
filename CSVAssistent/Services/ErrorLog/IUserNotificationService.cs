using System;
using System.Collections.Generic;
using System.Text;

namespace CSVAssistent.Services.ErrorLog
{
    public interface IUserNotificationService
    {
        void ShowInfo(string message, string title = "Info");
        void ShowWarning(string message, string title = "Warnung");
        void ShowError(string message, string title = "Fehler");
    }

}
