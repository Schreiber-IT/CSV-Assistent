using System;
using System.Collections.Generic;
using System.Text;

namespace CSVAssistent.Services.ErrorLog
{
    public interface IErrorService
    {
        void HandleException(
            Exception ex,
            string context = null,
            bool showToUser = true,
            bool isExpected = false);

        IEnumerable<ErrorLogEntry> GetLastErrors(int count = 100);
    }
}
