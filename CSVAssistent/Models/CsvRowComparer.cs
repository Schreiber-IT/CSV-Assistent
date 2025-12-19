using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CSVAssistent.Models
{
    public sealed class CsvRowComparer : IComparer
    {
        private readonly string _column;
        private readonly bool _ascending;

        public CsvRowComparer(string column, bool ascending)
        {
            _column = column;
            _ascending = ascending;
        }

        public int Compare(object? x, object? y)
        {
            if (x is not CsvRow left || y is not CsvRow right)
            {
                return 0;
            }

            var result = string.Compare(left[_column], right[_column], StringComparison.OrdinalIgnoreCase);
            return _ascending ? result : -result;
        }
    }
}
