using System;
using System.Collections.Generic;
using System.Text;

namespace CSVAssistent.Services.ErrorLog
{
    public class ErrorService : IErrorService
    {
        private readonly ILogService _log;
        private readonly IUserNotificationService _notification;

        public ErrorService(ILogService log, IUserNotificationService notification)
        {
            _log = log;
            _notification = notification;
        }

        public void HandleException(
            Exception ex,
            string context = null,
            bool showToUser = true,
            bool isExpected = false)
        {
            if (isExpected)
            {
                _log.Warn($"Erwarteter Fehler: {ex.Message}", context);
            }
            else
            {
                _log.Error("Unerwarteter Fehler", ex, context);
            }

            if (!showToUser)
                return;

            string userMessage = string.IsNullOrWhiteSpace(context)
                ? "Es ist ein Fehler aufgetreten."
                : $"Bei folgendem Vorgang ist ein Fehler aufgetreten:\n{context}";

            _notification.ShowError(userMessage);
        }

        public IEnumerable<ErrorLogEntry> GetLastErrors(int count = 100)
        {
            // Wenn der Logger lesen kann, gib das weiter
            if (_log is IErrorLogReader reader)
            {
                return reader.GetLast(count);
            }

            // Fallback, falls du irgendwann mal einen anderen Logger verwendest
            return Enumerable.Empty<ErrorLogEntry>();
        }
    }

}
