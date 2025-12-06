using System;
using System.Collections.Generic;
using System.Text;

namespace CSVAssistent.Services.ErrorLog
{
    public interface ILogService
    {
        void Info(string message, string context = null);
        void Warn(string message, string context = null);
        void Error(string message, Exception ex = null, string context = null);
    }

}
