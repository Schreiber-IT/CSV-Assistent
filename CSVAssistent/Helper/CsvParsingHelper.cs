using System;
using System.Collections.Generic;
using System.Text;

namespace CSVAssistent.Helper
{
    public static class CsvParsingHelper
    {
        private static readonly char[] DefaultDelimiters = { ';', ',', '\t', '|' };

        public static char DetectDelimiter(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return ';';
            }

            char bestDelimiter = DefaultDelimiters[0];
            var bestCount = -1;

            foreach (var delimiter in DefaultDelimiters)
            {
                var count = CountDelimiter(line, delimiter);
                if (count > bestCount)
                {
                    bestCount = count;
                    bestDelimiter = delimiter;
                }
            }

            return bestDelimiter;
        }

        private static int CountDelimiter(string line, char delimiter)
        {
            var count = 0;
            var inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                var ch = line[i];
                if (ch == '"')
                {
                    var isEscapedQuote = inQuotes && i + 1 < line.Length && line[i + 1] == '"';
                    if (isEscapedQuote)
                    {
                        i++;
                        continue;
                    }

                    inQuotes = !inQuotes;
                    continue;
                }

                if (!inQuotes && ch == delimiter)
                {
                    count++;
                }
            }

            return count;
        }

        public static List<string> SplitLine(string line, char delimiter)
        {
            var values = new List<string>();
            if (line == null)
            {
                return values;
            }

            var current = new StringBuilder();
            var inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                var ch = line[i];
                if (ch == '"')
                {
                    var isEscapedQuote = inQuotes && i + 1 < line.Length && line[i + 1] == '"';
                    if (isEscapedQuote)
                    {
                        current.Append('"');
                        i++;
                        continue;
                    }

                    inQuotes = !inQuotes;
                    continue;
                }

                if (ch == delimiter && !inQuotes)
                {
                    values.Add(current.ToString());
                    current.Clear();
                    continue;
                }

                current.Append(ch);
            }

            values.Add(current.ToString());
            return values;
        }
    }
}
