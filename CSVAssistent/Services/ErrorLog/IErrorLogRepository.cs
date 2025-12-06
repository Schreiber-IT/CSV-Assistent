using System;
using System.Collections.Generic;
using System.Text;

namespace CSVAssistent.Services.ErrorLog
{
    public interface IErrorLogRepository
    {
        IEnumerable<ErrorLogEntry> GetLast(int count = 100);
        IEnumerable<ErrorLogEntry> GetByLevel(string level);
        IEnumerable<ErrorLogEntry> Search(string text);
    }

}
