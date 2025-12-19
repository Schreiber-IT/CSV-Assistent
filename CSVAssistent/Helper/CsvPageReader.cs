using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSVAssistent.Helper
{
    public static class CsvPageReader
    {
        /// <summary>
        /// Liest eine Seite aus einer CSV-Datei (streamend).
        /// PageIndex ist 1-basiert (1 = erste Seite).
        /// </summary>
        public static async Task<CsvPage> ReadPageAsync(
            string filePath,
            long pageIndex,
            int pageSize,
            char separator = ';',
            bool hasHeader = true,
            Encoding? encoding = null,
            CancellationToken token = default)
        {
            if (pageIndex < 1) throw new ArgumentOutOfRangeException(nameof(pageIndex));
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

            encoding ??= Encoding.UTF8;

            string[]? headers = null;
            var rows = new List<string[]>(capacity: pageSize);

            long startRow = (pageIndex - 1) * (long)pageSize; // 0-basiert, nur Datenzeilen
            long skippedDataRows = 0;

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 1 << 16);
            using var reader = new StreamReader(fs, encoding, detectEncodingFromByteOrderMarks: true);

            // Header lesen (optional)
            if (hasHeader)
            {
                token.ThrowIfCancellationRequested();
                var headerLine = await reader.ReadLineAsync();
                if (headerLine == null)
                    return new CsvPage(Array.Empty<string>(), rows);

                headers = ParseCsvLine(headerLine, separator);
            }

            // Datenzeilen bis zur gewünschten Startzeile überspringen
            while (skippedDataRows < startRow)
            {
                token.ThrowIfCancellationRequested();
                var line = await reader.ReadLineAsync();
                if (line == null) break; // Datei zu Ende
                skippedDataRows++;
            }

            // PageSize Zeilen einlesen
            while (rows.Count < pageSize)
            {
                token.ThrowIfCancellationRequested();
                var line = await reader.ReadLineAsync();
                if (line == null) break;

                rows.Add(ParseCsvLine(line, separator));
            }

            // Falls kein Header: künstliche Spaltennamen aus erster Datenzeile ableiten
            if (!hasHeader)
            {
                int colCount = rows.Count > 0 ? rows[0].Length : 0;
                headers = new string[colCount];
                for (int i = 0; i < colCount; i++)
                    headers[i] = $"Col{i + 1}";
            }

            return new CsvPage(headers ?? Array.Empty<string>(), rows);
        }

        /// <summary>
        /// Einfacher CSV-Line-Parser mit Quotes (ohne Multi-Line-Felder).
        /// </summary>
        public static string[] ParseCsvLine(string line, char separator, char quote = '"')
        {
            var result = new List<string>();
            var sb = new StringBuilder();

            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == quote)
                {
                    // "" innerhalb von Quotes -> "
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == quote)
                    {
                        sb.Append(quote);
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == separator && !inQuotes)
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }

            result.Add(sb.ToString());
            return result.ToArray();
        }
    }

    public record CsvPage(string[] Headers, List<string[]> Rows);

}
