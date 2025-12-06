using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

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
            Write("INFO", message);
        }

        public void Warn(string message, string? context = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(context);
            Write("WARN", message);
        }
        public void Error(string message, Exception? ex = null, string? context = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(context);
            var fullMessage = ex == null ? message : $"{message}{Environment.NewLine}{ex}";
            Write("ERROR", fullMessage);
        }

        private void Write(string level, string message)
        {
            var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
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
