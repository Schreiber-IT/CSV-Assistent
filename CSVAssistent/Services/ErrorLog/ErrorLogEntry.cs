using System;
using System.Collections.Generic;
using System.Text;

namespace CSVAssistent.Services.ErrorLog
{
    public class ErrorLogEntry
    {
        // Auto-Increment in LiteDB
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string? Level { get; set; }          // INFO, WARN, ERROR
        public string? Message { get; set; }        // Kurzbeschreibung
        public string? Context { get; set; }        // z.B. "Datei speichern"

        public string? ExceptionType { get; set; }  // z.B. "System.NullReferenceException"
        public string? ExceptionMessage { get; set; }
        public string? StackTrace { get; set; }
    }
}
