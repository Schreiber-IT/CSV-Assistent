using CSVAssistent.Core;
using CSVAssistent.Helper;
using CSVAssistent.Models;
using CSVAssistent.Services;
using CSVAssistent.Services.Dialog;
using CSVAssistent.Services.ErrorLog;
using CSVAssistent.Services.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.IO;
using System.Windows.Data;

namespace CSVAssistent.ViewModel
{
    public class FileInfoViewModel : BaseViewModel
    {
        private readonly IWindowService _windowService;
        private readonly IErrorService _errorService;
        private readonly ISettingsService _settingsService;
        private readonly IFileDialogService _dialogService;

        private readonly AssignmentViewModel _assignmentViewModel;

        public ObservableCollection<string> ColumnNames { get; } = new();
        public ObservableCollection<string> SearchColumns { get; } = new();

        private ObservableCollection<CsvRow> _rows = new();
        public ObservableCollection<CsvRow> Rows
        {
            get => _rows;
            set
            {
                if (SetProperty(ref _rows, value))
                {
                    FilteredRows = CollectionViewSource.GetDefaultView(Rows);
                    FilteredRows.Filter = FilterRows;
                }
            }
        }

        public ICollectionView FilteredRows { get; private set; }

        public RelayCommand SearchCommand { get; }
        public RelayCommand ResetSearchCommand { get; }
        public RelayCommand SortCommand { get; }
        public RelayCommand OpenAssignmentCommand { get; }

        private const string AllColumnsLabel = "Alle Spalten";
        private string _searchTerm = string.Empty;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (SetProperty(ref _searchTerm, value))
                {
                    FilteredRows.Refresh();
                }
            }
        }

        private string _selectedSearchColumn = AllColumnsLabel;
        public string SelectedSearchColumn
        {
            get => _selectedSearchColumn;
            set
            {
                if (SetProperty(ref _selectedSearchColumn, value))
                {
                    FilteredRows.Refresh();
                }
            }
        }

        private string? _currentSortColumn;
        private bool _sortAscending = true;
        public string Separator = "";
        private FileEntry? _csvfile;
        public FileEntry? CsvFile
        {
            get => _csvfile;
            set => SetProperty(ref _csvfile, value);
        }

        public FileInfoViewModel()
        {
            _windowService = ServiceLocator.WindowService;
            _settingsService = ServiceLocator.SettingsService;
            _errorService = ServiceLocator.ErrorService;
            _dialogService = ServiceLocator.FileDialogService;

            _assignmentViewModel = new AssignmentViewModel();

            SearchCommand = new RelayCommand(_ => FilteredRows?.Refresh());
            ResetSearchCommand = new RelayCommand(_ => ResetSearch());
            SortCommand = new RelayCommand(column => ApplySort(column as string), column => column is string);
            OpenAssignmentCommand = new RelayCommand(_ => OpenAssignment());
        }

        private List<string> ReadColumns(FileEntry file)
        {
            if (!System.IO.File.Exists(file.FullPath))
                return new List<string>();

            var header = System.IO.File.ReadLines(file.FullPath).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(header))
                return new List<string>();

            var separator = CsvParsingHelper.DetectDelimiter(header);
            return CsvParsingHelper.SplitLine(header, separator)
                .Select((c, index) =>
                {
                    var trimmed = c.Trim();
                    return string.IsNullOrWhiteSpace(trimmed)
                        ? $"Column{index + 1}"
                        : trimmed;
                })
                .ToList();
        }

        public void OpenAssignment()
        {
            if (CsvFile == null) return;
            if (ColumnNames.Count <= 0) return;
            try
            {
                var columns = CsvFile.FullPath != null ? ReadColumns(CsvFile) : new List<string>();
                var fileName = CsvFile.FullPath ?? "Keine Datei ausgewählt";
                _assignmentViewModel.LoadColumns(columns, fileName);
                _windowService.ShowDialog(_assignmentViewModel);

                // Status aktualisieren: mind. eine Zuordnung mit SourceColumn → Status = 1
                if (CsvFile != null)
                {
                    var assigned = _assignmentViewModel.Mappings.Any(m => m.SourceColumnIndex.HasValue);
                    CsvFile.Status = assigned ? 1 : 0;
                }
            }
            catch (Exception ex)
            {
                _errorService.HandleException(
                    ex,
                    context: "OpenAssignment",
                    showToUser: true,
                    isExpected: false);
            }
        }
        public async Task Load(FileEntry file)
        {
            CsvFile = file;
            await LoadCsvDynamic(file.FullPath);
        }

        private async Task LoadCsvDynamic(string path)
        {
            ColumnNames.Clear();
            SearchColumns.Clear();

            if (!File.Exists(path)) return;

            var parsedData = await Task.Run(() =>
            {
                var lines = File.ReadAllLines(path);
                if (lines.Length == 0) return (new List<string>(), new List<CsvRow>());

                var delimiter = CsvParsingHelper.DetectDelimiter(lines[0]);
                var headers = CsvParsingHelper.SplitLine(lines[0], delimiter);

                var columnNames = new List<string>();
                foreach (var h in headers)
                {
                    var header = string.IsNullOrWhiteSpace(h) ? $"Column{columnNames.Count}" : h;
                    columnNames.Add(header);
                }

                var rows = new List<CsvRow>();
                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;
                    var parts = CsvParsingHelper.SplitLine(lines[i], delimiter);
                    var dict = new Dictionary<string, string>();
                    for (int c = 0; c < columnNames.Count; c++)
                        dict[columnNames[c]] = c < parts.Count ? parts[c] : string.Empty;
                    rows.Add(new CsvRow(dict));
                }

                return (columnNames, rows);
            });

            var (columnNames, rows) = parsedData;

            foreach (var c in columnNames)
                ColumnNames.Add(c);

            SearchColumns.Add(AllColumnsLabel);
            foreach (var h in columnNames)
                SearchColumns.Add(h);

            SelectedSearchColumn = AllColumnsLabel;
            SearchTerm = string.Empty;

            Rows = new ObservableCollection<CsvRow>(rows);

            ApplySort(ColumnNames.FirstOrDefault());
            FilteredRows.Refresh();
        }

        private bool FilterRows(object obj)
        {
            if (obj is not CsvRow row)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                return true;
            }

            var term = SearchTerm.Trim();
            var comparison = StringComparison.OrdinalIgnoreCase;

            if (SelectedSearchColumn == AllColumnsLabel)
            {
                return ColumnNames.Any(c => row[c].Contains(term, comparison));
            }

            return row[SelectedSearchColumn].Contains(term, comparison);
        }

        private void ResetSearch()
        {
            SearchTerm = string.Empty;
            SelectedSearchColumn = AllColumnsLabel;
            FilteredRows.Refresh();
        }

        private void ApplySort(string? columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName))
            {
                return;
            }

            if (_currentSortColumn == columnName)
            {
                _sortAscending = !_sortAscending;
            }
            else
            {
                _currentSortColumn = columnName;
                _sortAscending = true;
            }

            if (FilteredRows is ListCollectionView view)
            {
                view.CustomSort = new CsvRowComparer(columnName, _sortAscending);
            }
            FilteredRows.Refresh();
        }
    }
}
