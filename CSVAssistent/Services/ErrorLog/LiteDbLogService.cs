using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDB;

namespace CSVAssistent.Services.ErrorLog
{
    public class LiteDbLogService : ILogService, IErrorLogReader, IDisposable
    {
        private readonly LiteDatabase _db;
        private readonly ILiteCollection<ErrorLogEntry> _collection;

        public LiteDbLogService(string connectionString)
        {
            // z.B. "Filename=logs.db;Connection=shared;"
            _db = new LiteDatabase(connectionString);
            _collection = _db.GetCollection<ErrorLogEntry>("error_logs");

            _collection.EnsureIndex(x => x.Timestamp);
            _collection.EnsureIndex(x => x.Level);
            _collection.EnsureIndex(x => x.Context);
        }

        public void Info(string message, string context = null)
            => Write("INFO", message, null, context);

        public void Warn(string message, string context = null)
            => Write("WARN", message, null, context);

        public void Error(string message, Exception ex = null, string context = null)
            => Write("ERROR", message, ex, context);

        public IEnumerable<ErrorLogEntry> GetLast(int count = 100)
        {
            // Neueste zuerst
            return _collection.Query()
                .OrderByDescending(x => x.Timestamp)
                .Limit(count)
                .ToList();
        }

        private void Write(string level, string message, Exception ex, string context)
        {
            var entry = new ErrorLogEntry
            {
                Timestamp = DateTime.Now,
                Level = level,
                Message = message,
                Context = context,
                ExceptionType = ex?.GetType().FullName,
                ExceptionMessage = ex?.Message,
                StackTrace = ex?.ToString() // enthält meist StackTrace + Message
            };

            try
            {
                _collection.Insert(entry);
            }
            catch
            {
                // Logging darf niemals die App abschießen – Fehler hier bewusst ignorieren
            }
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}
