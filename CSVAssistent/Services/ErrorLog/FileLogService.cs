using System;
using System.Diagnostics;
using System.IO;

namespace CSVAssistent.Services.ErrorLog
{
    public class FileLogService : ILogService
    {
        private readonly string _logFilePath;

        public FileLogService(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public void Info(string message, string? context = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(context);
            Write("INFO", message, context);
        }

        public void Warn(string message, string? context = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(context);
            Write("WARN", message, context);
        }
        public void Error(string message, Exception? ex = null, string? context = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(context);
            var fullMessage = ex == null ? message : $"{message}{Environment.NewLine}{ex}";
            Write("ERROR", fullMessage, context);
        }

        private void Write(string level, string message, string? context)
        {
            var contextSuffix = string.IsNullOrWhiteSpace(context) ? string.Empty : $" [{context}]";
            var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}]{contextSuffix} {message}";
            Debug.WriteLine(line);

            try
            {
                File.AppendAllText(_logFilePath, line + Environment.NewLine);
            }
            catch
            {
                // Nichts tun – Logging darf die App nicht abschießen
            }
        }
    }
}
