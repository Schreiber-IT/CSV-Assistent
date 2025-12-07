using System;
using System.ComponentModel;
using System.Globalization; // Neu hinzufügen
using System.Runtime.CompilerServices;

namespace CSVAssistent.Models
{
    public class FileEntry : INotifyPropertyChanged
    {
        public string Name { get; set; } = "";
        public string FullPath { get; set; } = "";

        private int _status = 0; // 0 = nicht zugewiesen, 1 = zugewiesen

        public long Lines { get; set; }
        
        // Neue Property für formatierte Zeilenanzahl
        public string FormattedLines => Lines.ToString("N0", new CultureInfo("de-DE"));

        public int Status
        {
            get => _status;
            set
            {
                if (_status == value) return;
                _status = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
