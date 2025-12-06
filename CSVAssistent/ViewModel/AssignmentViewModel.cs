using CSVAssistent.Core;
using CSVAssistent.Models;
using CSVAssistent.Services;
using CSVAssistent.Services.Dialog;
using CSVAssistent.Services.ErrorLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Windows.Input;

namespace CSVAssistent.ViewModel
{
    public class AssignmentViewModel : BaseViewModel
    {
        private readonly IColumnAssignmentService _assignmentService;
        private readonly IFileDialogService _dialogService;
        private readonly IErrorService _errorService;

        private string? _selectedFileName;
        private string _newTargetColumnName = string.Empty;

        private static readonly ColumnOption EmptyColumn = new(-1, string.Empty);

        public ObservableCollection<ColumnAssignment> Mappings => _assignmentService.Mappings;

        public ObservableCollection<ColumnOption> AvailableColumns { get; } = new();

        public IEnumerable<ColumnOption> AvailableColumnsWithEmpty => new[] { EmptyColumn }.Concat(AvailableColumns);

        public string? SelectedFileName
        {
            get => _selectedFileName;
            set
            {
                if (_selectedFileName == value) return;
                _selectedFileName = value;
                OnPropertyChanged();
            }
        }

        public string NewTargetColumnName
        {
            get => _newTargetColumnName;
            set
            {
                if (_newTargetColumnName == value) return;
                _newTargetColumnName = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddMappingCommand { get; }
        public ICommand SaveAssignmentsCommand { get; }
        public ICommand LoadAssignmentsCommand { get; }

        public event EventHandler? AssignmentsChanged;

        public AssignmentViewModel(IColumnAssignmentService? assignmentService = null)
        {
            _assignmentService = assignmentService ?? ServiceLocator.ColumnAssignmentService;
            _dialogService = ServiceLocator.FileDialogService;
            _errorService = ServiceLocator.ErrorService;
            AddMappingCommand = new RelayCommand(_ => AddMapping(), _ => !string.IsNullOrWhiteSpace(NewTargetColumnName));
            SaveAssignmentsCommand = new RelayCommand(_ => SaveAssignments(), _ => Mappings.Any());
            LoadAssignmentsCommand = new RelayCommand(_ => LoadAssignments());
        }

        public void LoadColumns(IEnumerable<string> columns, string fileName)
        {
            AvailableColumns.Clear();
            foreach (var column in columns.Select((name, index) => new { name, index }))
            {
                var trimmed = column.name.Trim();
                var displayName = string.IsNullOrWhiteSpace(trimmed)
                    ? $"Column{column.index + 1}"
                    : trimmed;

                AvailableColumns.Add(new ColumnOption(column.index, displayName));
            }

            SelectedFileName = fileName;
            _assignmentService.SetCurrentFile(fileName);
            OnPropertyChanged(nameof(AvailableColumnsWithEmpty));
        }

        private void AddMapping()
        {
            _assignmentService.AddMapping(NewTargetColumnName);
            NewTargetColumnName = string.Empty;
        }

        private void SaveAssignments()
        {
            try
            {
                var file = _dialogService.SaveFile();
                if (string.IsNullOrWhiteSpace(file))
                    return;

                _assignmentService.SaveToFile(file);
            }
            catch (Exception ex)
            {
                _errorService.HandleException(ex, nameof(SaveAssignments), true, false);
            }
        }

        private void LoadAssignments()
        {
            try
            {
                var file = _dialogService.OpenFile("JSON Dateien (*.json)|*.json|Alle Dateien (*.*)|*.*");
                if (string.IsNullOrWhiteSpace(file))
                    return;

                _assignmentService.LoadFromFile(file);
                OnPropertyChanged(nameof(Mappings));
                OnPropertyChanged(nameof(AvailableColumnsWithEmpty));
                OnAssignmentsChanged();
            }
            catch (Exception ex)
            {
                _errorService.HandleException(ex, nameof(LoadAssignments), true, false);
            }
        }

        private void OnAssignmentsChanged()
        {
            AssignmentsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
