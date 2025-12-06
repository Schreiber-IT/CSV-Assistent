using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CSVAssistent.Models
{
    public class ColumnAssignment : INotifyPropertyChanged
    {
        private string _targetColumn = string.Empty;
        private int? _sourceColumnIndex;

        public string TargetColumn
        {
            get => _targetColumn;
            set
            {
                if (_targetColumn == value) return;
                _targetColumn = value;
                OnPropertyChanged();
            }
        }

        public int? SourceColumnIndex
        {
            get => _sourceColumnIndex;
            set
            {
                var normalized = value.HasValue && value.Value < 0 ? null : value;
                if (_sourceColumnIndex == normalized) return;
                _sourceColumnIndex = normalized;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedIndex)); // Notify für SelectedIndex
            }
        }

        // Neue Property für die ComboBox-Binding
        public int SelectedIndex
        {
            get => SourceColumnIndex ?? -1;
            set
            {
                SourceColumnIndex = value == -1 ? null : value;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
