using System;
using System.Collections.Generic;
using System.Text;

namespace CSVAssistent.Services.ErrorLog
{
    public interface IErrorLogReader
    {
        IEnumerable<ErrorLogEntry> GetLast(int count = 100);
    }

}
