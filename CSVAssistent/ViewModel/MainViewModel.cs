using CSVAssistent.Core;
using CSVAssistent.Helper;
using CSVAssistent.Models;
using CSVAssistent.Services;
using CSVAssistent.Services.Dialog;
using CSVAssistent.Services.ErrorLog;
using CSVAssistent.Services.Settings;
using CSVAssistent.Services.Theming;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CSVAssistent.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IWindowService _windowService;
        private readonly IErrorService _errorService;
        private readonly ISettingsService _settingsService;
        private readonly IFileDialogService _dialogService;
        private readonly IThemeService _themeService;
        private readonly IColumnAssignmentService _assignmentService;

        private readonly InfoDialogViewModel _infoDialogViewModel;
        private readonly SettingsDialogViewModel _settingsDialogViewModel;
        private readonly HelpViewModel _helpViewModel;
        private readonly FileInfoViewModel _fileInfoViewModel;
        public AssignmentViewModel _assignmentViewModel { get; }
        private readonly SplitViewModel _splitViewModel;


        public ObservableCollection<FileEntry> Files { get; } = new();

        
        private int _menuiconsize;
        public int MenuIconSize
        {
            get => _menuiconsize;
            set
            {
                if (_menuiconsize == value) return;
                _menuiconsize = value;
                _settingsService.SetInt("MenuIconSize", value);
                OnPropertyChanged();
            }
        }

        private int _progressValue;
        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                if (_progressValue == value) return;
                _progressValue = value;
                OnPropertyChanged();
            }
        }

        private int _menutextsize;
        public int MenuTextSize
        {
            get => _menutextsize;
            set
            {
                if (_menutextsize == value) return;
                _menutextsize = value;
                _settingsService.SetInt("MenuTextSize", value);
                OnPropertyChanged();
            }
        }
        
        private bool _exportReady;
        public bool ExportReady
        {
            get => _exportReady;
            private set
            {
                if (SetProperty(ref _exportReady, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }
        

        private string? _footerInfo;
        public string? FooterInfo
        {
            get => _footerInfo;
            set
            {
                if (_footerInfo == value) return;
                _footerInfo = value;
                OnPropertyChanged();
            }
        }

        private string? _activeTheme;
        public string? ActiveTheme
        {
            get => _activeTheme;
            set
            {
                if (_activeTheme == value) return;
                _activeTheme = value;
                OnPropertyChanged();
            }
        }

        private FileEntry? _selectedFile;
        public FileEntry? SelectedFile
        {
            get => _selectedFile;
            set
            {
                if (_selectedFile == value) return;
                _selectedFile = value;
                OnPropertyChanged();
            }
        }

        public WindowState _windowState { get; set; }
        public WindowState WindowState
        {
            get => _windowState;
            set
            {
                if (_windowState != value)
                {
                    _windowState = value;
                    OnPropertyChanged(); // aus deinem ViewModelBase
                }
            }
        }

        public ICommand SettingsDialogCommand { get; }
        public ICommand InfoDialogCommand { get; }
        public ICommand OpenFileInfoCommand { get; }
        public ICommand AddFileCommand { get; }
        public ICommand AddFolderCommand { get; }
        public ICommand DeleteFileCommand { get; }
        public ICommand DeleteAllFilesCommand { get; }
        public ICommand ScanAllFilesCommand { get; }
        public ICommand ExportDataCommand { get; }
        public ICommand HelpCommand { get; }
        public ICommand ChangeThemeCommand { get; }
        public ICommand AddFilesDropCommand { get; }
        public ICommand OpenAssignmentCommand { get; }
        public ICommand HandleDoubleClickCommand { get; }
        public ICommand SplitFileCommand { get; }
        
        // Definitionen
        private string windowState = "MainWindowWindowState";

        public MainViewModel()
        {
            _windowService = ServiceLocator.WindowService;
            _settingsService = ServiceLocator.SettingsService;
            _errorService = ServiceLocator.ErrorService;
            _dialogService = ServiceLocator.FileDialogService;
            _themeService = ServiceLocator.ThemeService;
            _assignmentService = ServiceLocator.ColumnAssignmentService;

            _infoDialogViewModel = new InfoDialogViewModel();
            InfoDialogCommand = new RelayCommand(_ => OpenInfoDialog());

            _settingsDialogViewModel = new SettingsDialogViewModel();
            SettingsDialogCommand = new RelayCommand(_ => OpenSettingsDialog());

            // Live-Update: auf Änderungen im AppSettings-VM reagieren
            _settingsDialogViewModel.AppSettingsViewModel.PropertyChanged += AppSettings_PropertyChanged;

            _helpViewModel = new HelpViewModel();
            HelpCommand = new RelayCommand(_ => ShowHelp());

            _fileInfoViewModel = new FileInfoViewModel();
            OpenFileInfoCommand = new RelayCommand(_ => ShowFileInfo(), _ => SelectedFile != null);

            _splitViewModel = new SplitViewModel();
            SplitFileCommand = new RelayCommand(_ => SplitFile(), _ => SelectedFile != null);
            
            _assignmentViewModel = new AssignmentViewModel(_assignmentService);
            _assignmentViewModel.AssignmentsChanged += AssignmentViewModel_AssignmentsChanged;
            OpenAssignmentCommand = new RelayCommand(_ => OpenAssignment(), _ => Files.Count >= 0);

            HandleDoubleClickCommand = new RelayCommand(_ => HandleDoubleClick(), _ => SelectedFile != null);

            ChangeThemeCommand = new RelayCommand(_ => ChangeTheme());

            DeleteFileCommand = new RelayCommand(_ => DeleteFile(), _ => SelectedFile != null);
            DeleteAllFilesCommand = new RelayCommand(_ => DeleteAllFiles(), _ => Files.Count >= 1);
            ScanAllFilesCommand = new RelayCommand(_ => ScanAllFiles(), _ => Files.Count >= 1);
            ExportDataCommand = new RelayCommand(_ => Export(), _ => ExportReady == true);
            AddFilesDropCommand = new RelayCommand(p => AddFilesFromDrop(p), p => p is IEnumerable<string>);

            AddFileCommand = new RelayCommand(_ => AddSingleFile());
            AddFolderCommand = new RelayCommand(_ => AddFolder());

            LoadSettings();
        }

        public void SplitFile()
        {
            try
            {
                var file = SelectedFile;
                if (file == null) return;
                _splitViewModel.Load(file);
                _windowService.ShowDialog(_splitViewModel);
                ScanAllFiles();
            }
            catch (Exception ex)
            {
                _errorService.HandleException(
                    ex,
                    context: "SplitFile, MainWindow",
                    showToUser: true,
                    isExpected: false);
            }
        }

        public static long CountLines(string path)
        {
            const int bufferSize = 1024 * 1024; // 1 MB-Buffer
            byte[] buffer = new byte[bufferSize];

            long lineCount = 0;
            bool lastByteWasNewline = false;

            var fileInfo = new FileInfo(path);
            long fileLength = fileInfo.Length;

            using (var stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < bytesRead; i++)
                    {
                        if (buffer[i] == (byte)'\n')
                        {
                            lineCount++;
                        }
                    }

                    lastByteWasNewline = buffer[bytesRead - 1] == (byte)'\n';
                }
            }

            // Falls die letzte Zeile nicht mit '\n' endet, noch eine Zeile dazuzählen
            if (fileLength > 0 && !lastByteWasNewline)
            {
                lineCount++;
            }

            return lineCount;
        }

        private void ShowHelp()
        {
            try
            {
                _windowService.Show(_helpViewModel);
            }
            catch (Exception ex)
            {
                _errorService.HandleException(
                    ex,
                    context: "ShowHelp",
                    showToUser: true,
                    isExpected: false);
            }
        }

        private void OpenAssignment()
        {
            try
            {
                var columns = SelectedFile != null ? ReadColumns(SelectedFile) : new List<string>();
                var fileName = SelectedFile?.FullPath ?? "Keine Datei ausgewählt";
                _assignmentViewModel.LoadColumns(columns, fileName);
                _windowService.ShowDialog(_assignmentViewModel);

                if (SelectedFile != null)
                {
                    var assigned = _assignmentViewModel.Mappings.Any(m => m.SourceColumnIndex.HasValue);
                    SelectedFile.Status = assigned ? 1 : 0;
                }
                ScanAllFiles();
            }
            catch (Exception ex)
            {
                _errorService.HandleException(ex, "OpenAssignment", true, false);
            }
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

        private void AddFilesFromDrop(object? parameter)
        {
            try
            {
                if (parameter is not IEnumerable<string> files) return;
                foreach (var f in files)
                {
                    if (System.IO.File.Exists(f))
                        AddFileEntry(f);
                }
                ScanAllFiles();
            }
            catch (Exception ex)
            {
                _errorService.HandleException(ex, "AddFilesFromDrop", true, false);
            }
        }

        public void OnWindowClosing()
        {
            if (WindowState.ToString() == "Minimized")
            {
                WindowState = WindowState.Normal;
                _settingsService.SetString(windowState, WindowState.Normal.ToString());
            }
            else
            {
                _settingsService.SetString(windowState, WindowState.ToString());
            }
            _settingsService.SetString("Theme", _themeService.CurrentThemeId);
        }

        private void Export()
        {
            try
            {
                if (Files.Count == 0) return;

                var targetPath = _dialogService.SaveFileAs();
                if (string.IsNullOrWhiteSpace(targetPath))
                    return;

                var hasAssignments = _assignmentService.Mappings.Any();

                if (!hasAssignments)
                {
                    ExportWithoutAssignments(targetPath);
                }
                else
                {
                    ExportWithAssignments(targetPath);
                }

                var assignmentFilePath = DetermineAssignmentFilePath(targetPath);
                _assignmentService.SaveToFile(assignmentFilePath);

                System.Windows.MessageBox.Show(
                    $"Daten erfolgreich exportiert nach:\n{targetPath}",
                    "Export abgeschlossen",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _errorService.HandleException(ex, "Export", true, false);
            }
        }

        private static string DetermineAssignmentFilePath(string exportPath)
        {
            var directory = Path.GetDirectoryName(exportPath) ?? string.Empty;
            var fileName = Path.GetFileName(exportPath);
            var baseName = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);

            if (string.Equals(extension, ".json", StringComparison.OrdinalIgnoreCase))
            {
                return Path.Combine(directory, $"{baseName}.1.json");
            }

            return Path.Combine(directory, $"{baseName}.json");
        }

        private void ExportWithoutAssignments(string targetPath)
        {
            var firstFile = Files.First();
            var firstLines = System.IO.File.ReadLines(firstFile.FullPath).ToList();
            if (firstLines.Count == 0)
                throw new InvalidOperationException("Die erste Datei enthält keine Daten.");

            var header = firstLines[0];

            using var writer = new StreamWriter(targetPath, false, Encoding.UTF8);
            writer.WriteLine(header);

            foreach (var file in Files)
            {
                var lines = System.IO.File.ReadLines(file.FullPath);
                var skipHeader = true;
                foreach (var line in lines)
                {
                    if (skipHeader)
                    {
                        skipHeader = false;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    writer.WriteLine(line);
                }
            }
        }

        private void ExportWithAssignments(string targetPath)
        {
            var targetColumns = _assignmentService.Mappings
                .Select(m => m.TargetColumn)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .ToList();

            if (targetColumns.Count == 0)
            {
                ExportWithoutAssignments(targetPath);
                return;
            }

            var separator = ';';

            using var writer = new StreamWriter(targetPath, false, Encoding.UTF8);
            writer.WriteLine(string.Join(separator, targetColumns));

            foreach (var file in Files)
            {
                _assignmentService.SetCurrentFile(file.FullPath);

                var sourceColumns = _assignmentService.Mappings
                    .ToDictionary(
                        m => m.TargetColumn,
                        m => m.SourceColumnIndex,
                        StringComparer.OrdinalIgnoreCase);

                var lines = System.IO.File.ReadLines(file.FullPath);
                var header = lines.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(header))
                    continue;

                var fileSeparator = CsvParsingHelper.DetectDelimiter(header);
                var headerColumns = CsvParsingHelper
                    .SplitLine(header, fileSeparator)
                    .Select(name => name.Trim())
                    .ToList();

                foreach (var line in lines.Skip(1))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var values = CsvParsingHelper.SplitLine(line, fileSeparator);

                    var outputValues = new List<string>();

                    foreach (var targetColumn in targetColumns)
                    {
                        var sourceColumn = sourceColumns.TryGetValue(targetColumn, out var source) ? source : null;

                        string? value = null;

                        if (sourceColumn.HasValue && sourceColumn.Value >= 0 && sourceColumn.Value < values.Count)
                        {
                            value = values[sourceColumn.Value];
                        }

                        if (string.IsNullOrEmpty(value))
                        {
                            var fallbackIdx = headerColumns.FindIndex(name =>
                                string.Equals(name, targetColumn, StringComparison.OrdinalIgnoreCase));

                            if (fallbackIdx >= 0 && fallbackIdx < values.Count)
                            {
                                value = values[fallbackIdx];
                            }
                        }

                        outputValues.Add(value ?? string.Empty);
                    }

                    writer.WriteLine(string.Join(separator, outputValues));
                }
            }
        }

        private void ScanAllFiles()
        {
            try
            {
                if (Files.Count == 0) return;

                if (Files.All(file => _assignmentService.HasAnyMapping(file.FullPath)))
                {
                    ExportReady = true;
                    return;
                }

                var referenceColumns = ReadColumns(Files.First());
                if (referenceColumns.Count == 0)
                {
                    ExportReady = false;
                    return;
                }

                bool allSameColumns = Files.All(file =>
                {
                    var columns = ReadColumns(file);
                    return columns.Count == referenceColumns.Count
                        && columns.SequenceEqual(referenceColumns, StringComparer.OrdinalIgnoreCase);
                });

                ExportReady = allSameColumns;
            }
            catch (Exception ex)
            {
                ExportReady = false;
                _errorService.HandleException(ex, "ScanAllFiles", true, false);
            }
        }

        private void DeleteAllFiles()
        {
            if (Files.Count == 0) return;
            var result = System.Windows.MessageBox.Show(
                $"Soll die Liste wirklich gelöscht werden ?\n Die Liste enthält {Files.Count} Dateien!",
                "Liste Löschen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                Files.Clear();
            }
            ScanAllFiles();
        }

        private void DeleteFile()
        {
            var file = SelectedFile;
            if (file == null) return;
            var result = System.Windows.MessageBox.Show(
                $"Soll die Datei aus der Liste gelöscht werden? \nPfad: {file.FullPath}",
                "Datei aus Liste Löschen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                Files.Remove(file);
            }
            ScanAllFiles();
        }

        private void AddSingleFile()
        {
            try
            {
                var path = _dialogService.OpenFile();
                if (path == null) return;
                if (!System.IO.File.Exists(path)) return;
                AddFileEntry(path);
                ScanAllFiles();
            }
            catch (FileNotFoundException ex)
            {
                _errorService.HandleException(
                    ex,
                    context: "AddSingleFile - Datei nicht gefunden, MainWindow",
                    showToUser: true,
                    isExpected: true);
            }
            catch (Exception ex)
            {
                _errorService.HandleException(
                    ex,
                    context: "AddSingleFile, MainWindow",
                    showToUser: true,
                    isExpected: false);
            }
        }

        // Hilfsmethode: baut FileEntry im Hintergrund
private FileEntry BuildFileEntry(string fullPath)
{
    // I/O im Hintergrund
    var lines = CountLines(fullPath);
    var name = Path.GetFileName(fullPath);
    var separator = string.Empty;

    var firstLine = System.IO.File.ReadLines(fullPath).FirstOrDefault();
    if (firstLine != null)
    {
        var del = CsvParsingHelper.DetectDelimiter(firstLine);
        separator = del.ToString();
    }

    var entry = new FileEntry
    {
        FullPath = fullPath,
        Name = name,
        Lines = lines,
        Separator = separator
    };
    entry.SetHash();
    return entry;
}

        private async Task AddFolderAsync()
        {
            try
            {
                var folder = _dialogService.OpenFolder();
                if (folder == null || !Directory.Exists(folder)) return;

                var folderInfo = new DirectoryInfo(folder);
                var csvFiles = folderInfo.GetFiles("*.csv", SearchOption.TopDirectoryOnly);
                var totalFiles = csvFiles.Length;
                if (totalFiles == 0)
                {
                    ProgressValue = 0;
                    return;
                }

                ProgressValue = 0;

                int count = 0;
                foreach (var file in csvFiles)
                {
                    // Baue das FileEntry im Hintergrund
                    var entry = await Task.Run(() => BuildFileEntry(file.FullName));

                    // Füge minimal auf dem UI-Thread hinzu
                    await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        if (!Files.Any(f => f.FullPath == entry.FullPath))
                        {
                            Files.Add(entry);
                        }
                    }, System.Windows.Threading.DispatcherPriority.Background);

                    count++;
                    var progress = (int)((count / (double)totalFiles) * 100);

                    // Fortschritt aktualisieren ohne Blockieren
                    await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        ProgressValue = progress;
                    }, System.Windows.Threading.DispatcherPriority.Background);

                    // UI Rendering ermöglichen
                    await Task.Yield();
                }

                ProgressValue = 0;
                ScanAllFiles();
            }
            catch (DirectoryNotFoundException ex)
            {
                _errorService.HandleException(
                    ex,
                    context: "AddFolder - Ordner nicht gefunden, MainWindow",
                    showToUser: true,
                    isExpected: true);
            }
            catch (Exception ex)
            {
                _errorService.HandleException(
                    ex,
                    context: "AddFolder, MainWindow",
                    showToUser: true,
                    isExpected: false);
            }
        }

        // Command auf neue async-Methode umstellen
        private async void AddFolder()
        {
            await AddFolderAsync();
        }

        private void AddFileEntry(string fullPath)
        {
            try
            {
                if (!System.IO.File.Exists(fullPath)) { return; }
                if (Files.Any(f => f.FullPath == fullPath)) { return; }

                // Count lines
                var lines = CountLines(fullPath);
                
                Files.Add(new FileEntry
                {
                    FullPath = fullPath,
                    Name = Path.GetFileName(fullPath),
                    Lines = lines
                });

                var addedFile = Files.Last();
                if (addedFile == null) return;

                // Detect delimiter
                var line = System.IO.File.ReadLines(fullPath).FirstOrDefault();
                if (line != null) {
                    var del = CsvParsingHelper.DetectDelimiter(line);
                    addedFile.Separator = del.ToString();
                }

                // Set hash for the added file
                addedFile.SetHash();
            }
            catch (FileNotFoundException ex)
            {
                _errorService.HandleException(
                    ex,
                    context: "AddFileEntry - Datei nicht gefunden, MainWindow",
                    showToUser: true,
                    isExpected: true);
            }
            catch (Exception ex)
            {
                _errorService.HandleException(
                    ex,
                    context: "AddFileEntry, MainWindow",
                    showToUser: true,
                    isExpected: false);
            }
        }

        private async void ShowFileInfo()
        {
            try
            {
                var file = SelectedFile;
                if (file == null) return;
                var rowlimitString = _settingsService.GetString(AppSettingsViewModel.RowLimitKey, "100_000");
                if (!int.TryParse(rowlimitString.Replace("_", ""), out var rowlimit))
                {
                    rowlimit = 100000;
                }

                if (file.Lines > rowlimit)
                {
                    var result = System.Windows.MessageBox.Show(
                        $"Die Datei hat das {rowlimitString} Zeilen Limit erreicht, \n" +
                        $"um einen Programmabsturz zu verhindern können wir das ganze leider nicht mehr anzeigen, \n" +
                        $"das zuweisen und exportieren funktioniert weiterhin!\n\n" +
                        $"Nutzen sie die Split-Funktion um die Datei in kleinere Dateien zu splitten",
                        $"Achtung, {rowlimitString} Zeilen Limit erreicht",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                } else {
                    FooterInfo = "Datei wird geladen!";
                    await _fileInfoViewModel.Load(file);
                    _windowService.Show(_fileInfoViewModel);
                    ScanAllFiles();
                    FooterInfo = "";
                }
            }
            catch (Exception ex)
            {
                _errorService.HandleException(
                    ex,
                    context: "ShowFileInfo, MainWindow",
                    showToUser: true,
                    isExpected: false);
            }
        }

        private void HandleDoubleClick()
        {
            try
            {
                var action = _settingsService.GetString(AppSettingsViewModel.DoubleClickSettingKey, "Viewer");

                switch (action)
                {
                    case "Zuordnungsliste":
                        OpenAssignment();
                        break;
                    case "Viewer_Neu":
                        ShowFileInfo();
                        break;
                    case "FileSplit":
                        SplitFile();
                        break;
                    default:
                        ShowFileInfo();
                        break;
                }
            }
            catch (Exception ex)
            {
                _errorService.HandleException(ex, "HandleDoubleClick", true, false);
            }
        }

        private void LoadSettings()
        {
            try
            {
                // --------------------------------------------------------------------------
                // Load Window State
                // --------------------------------------------------------------------------
                var ws = _settingsService.GetString(windowState, "Normal");
                switch (ws)
                {
                    case "Normal":
                        WindowState = WindowState.Normal;
                        break;
                    case "Maximized":
                        WindowState = WindowState.Maximized;
                        break;
                    case "Minimized":
                        WindowState = WindowState.Minimized;
                        break;
                    default:
                        WindowState = WindowState.Normal;
                        break;
                }
                ;
                // --------------------------------------------------------------------------

                // --------------------------------------------------------------------------
                // Load Theme
                // --------------------------------------------------------------------------
                var theme = _settingsService.GetString("Theme", "Dark");
                _themeService.ApplyTheme(theme);
                ActiveTheme = _themeService.CurrentThemeId;
                // --------------------------------------------------------------------------


                MenuIconSize = _settingsService.GetInt("MenuIconSize", 20);
                MenuTextSize = _settingsService.GetInt("MenuTextSize", 10);

            }
            catch (Exception ex)
            {
                _errorService.HandleException(
                    ex,
                    context: "MainWindow-LoadSettings",
                    showToUser: true,
                    isExpected: false);
            }
        }

        private void OpenInfoDialog()
        {
            try
            {
                _windowService.ShowDialog(_infoDialogViewModel);
            }
            catch (Exception ex)
            {
                _errorService.HandleException(
                    ex,
                    context: "OpenInfoDialog",
                    showToUser: true,
                    isExpected: false);
            }
        }

        private void OpenSettingsDialog()
        {
            try
            {
                _windowService.ShowDialog(_settingsDialogViewModel);
            }
            catch (Exception ex)
            {
                _errorService.HandleException(
                    ex,
                    context: "OpenSettingsDialog",
                    showToUser: true,
                    isExpected: false);
            }
        }

        private void ChangeTheme()
        {
            if (ActiveTheme == "Light")
            {
                _themeService.ApplyTheme("MidLight");
            }
            else if (ActiveTheme == "Dark")
            {
                _themeService.ApplyTheme("Light");
            }
            else
            {
                _themeService.ApplyTheme("Dark");
            }
            ActiveTheme = _themeService.CurrentThemeId;

        }

        private void AssignmentViewModel_AssignmentsChanged(object? sender, EventArgs e)
        {
            try
            {
                foreach (var entry in Files)
                {
                    entry.Status = _assignmentService.HasAnyMapping(entry.FullPath) ? 1 : 0;
                }
            }
            catch (Exception ex)
            {
                _errorService.HandleException(ex, "AssignmentViewModel_AssignmentsChanged", true, false);
            }
        }

        private void AppSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Synchronisiere Menügrößen sofort beim Slider-Bewegen
            if (e.PropertyName == nameof(AppSettingsViewModel.SelectedRowLimit))
            {
                // nichts für Menügrößen
            }
            else if (e.PropertyName == "MenuIconSize")
            {
                // Falls Sie MenuIconSize im AppSettingsViewModel hinzufügen:
                // MenuIconSize = _settingsService.GetInt("MenuIconSize", MenuIconSize);
                // Oder direkt aus dem VM casten:
                if (sender is AppSettingsViewModel vm)
                {
                    // optional: aus SettingsService lesen, da VM SetInt bereits aufruft
                    MenuIconSize = _settingsService.GetInt("MenuIconSize", MenuIconSize);
                }
            }
            else if (e.PropertyName == "MenuTextSize")
            {
                if (sender is AppSettingsViewModel vm)
                {
                    MenuTextSize = _settingsService.GetInt("MenuTextSize", MenuTextSize);
                }
            }
        }
    }

}
