using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CSVAssistent.Models
{
    public class CsvRow : INotifyPropertyChanged
    {
        private readonly Dictionary<string, string> _values;
        public CsvRow(Dictionary<string, string> values) => _values = values;

        public string this[string column] => _values.TryGetValue(column, out var v) ? v : string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;
        // Falls Werte später geändert werden sollen, PropertyChanged für Indexer auslösen:
        // public void Set(string col, string value) { _values[col] = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs($"Item[{col}]")); }
    }
}
