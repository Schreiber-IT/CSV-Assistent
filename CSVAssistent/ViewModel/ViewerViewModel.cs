using CSVAssistent.Core;
using CSVAssistent.Helper;
using CSVAssistent.Models;
using CSVAssistent.Services;
using CSVAssistent.Services.ErrorLog;
using CSVAssistent.Services.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace CSVAssistent.ViewModel
{
    public class ViewerViewModel : BaseViewModel
    {
        private readonly IWindowService _windowService;
        private readonly IErrorService _errorService;
        private readonly ISettingsService _settingsService;

        private CancellationTokenSource? _loadCts;

        public ICommand FirstPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand LastPageCommand { get; }
        public ICommand CancelCommand { get; }

        public bool IsPagingEnabled => TotalPages > 1;

        private DataTable? _pageTable;
        public DataView? PageView => _pageTable?.DefaultView;

        ObservableCollection<string[]> PageRows { get; set; } = new ObservableCollection<string[]>();

        private FileEntry? _viewerFile;
        public FileEntry? ViewerFile
        {
            get => _viewerFile;
            set
            {
                if (_viewerFile == value) return;
                _viewerFile = value;
                OnPropertyChanged();
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
        }

        private long _currentPageIndex;
        public long CurrentPageIndex
        {
            get => _currentPageIndex;
            set
            {
                if (_currentPageIndex == value) return;
                _currentPageIndex = value;
                OnPropertyChanged();
            }
        }

        private long _totalPages;
        public long TotalPages
        {
            get => _totalPages;
            set
            {
                if (_totalPages == value) return;
                _totalPages = value;
                OnPropertyChanged();
            }
        }

        private int _defaultPageSize;
        public int DefaultPageSize
        {
            get => _defaultPageSize;
            private set
            {
                if (_defaultPageSize == value) return;
                _defaultPageSize = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsPagingEnabled));
            }
        }

        public int PagingThreshold { get; set; } = 0;
        private string[]? _headers;


        public ViewerViewModel()
        {
            _windowService = ServiceLocator.WindowService;
            _settingsService = ServiceLocator.SettingsService;
            _errorService = ServiceLocator.ErrorService;


            FirstPageCommand = new RelayCommand(async _ => await GoToPageAsync(1), _ => IsPagingEnabled && CurrentPageIndex > 1);
            PrevPageCommand = new RelayCommand(async _ => await GoToPageAsync(CurrentPageIndex - 1), _ => IsPagingEnabled && CurrentPageIndex > 1);
            NextPageCommand = new RelayCommand(async _ => await GoToPageAsync(CurrentPageIndex + 1), _ => IsPagingEnabled && CurrentPageIndex < TotalPages);
            LastPageCommand = new RelayCommand(async _ => await GoToPageAsync(TotalPages), _ => IsPagingEnabled && CurrentPageIndex < TotalPages);
            CancelCommand = new RelayCommand(_ => Cancel(), _ => IsBusy);
        }
        private void Cancel()
        {
            _loadCts?.Cancel();
        }

        public async Task Load(FileEntry file)
        {
            ViewerFile = file;
            if (ViewerFile == null) return;

            var rowlimitString = _settingsService.GetString(AppSettingsViewModel.RowLimitKey, "100_000");
            if (!int.TryParse(rowlimitString.Replace("_", ""), out var rowlimit))
            {
                rowlimit = 100000;
            }

            var pagerowlimitString = _settingsService.GetString(AppSettingsViewModel.MaxRowKey, "100_000");
            if (!int.TryParse(pagerowlimitString.Replace("_", ""), out var pagerowlimit))
            {
                pagerowlimit = 100000;
            }

            PagingThreshold = rowlimit;
            DefaultPageSize = pagerowlimit;
            TotalPages = (long)Math.Ceiling((double)ViewerFile.Lines / pagerowlimit);
            CurrentPageIndex = 1;


            await GoToPageAsync(1);

        }
        private async Task GoToPageAsync(long pageIndex)
        {
            if (ViewerFile == null) return;
            if (TotalPages < 1) return;

            if (pageIndex < 1) pageIndex = 1;
            if (pageIndex > TotalPages) pageIndex = TotalPages;

            var oldindex = CurrentPageIndex;

            _loadCts?.Cancel();
            _loadCts?.Dispose();
            _loadCts = new CancellationTokenSource();
            IsBusy = true;

            try
            {
                CurrentPageIndex = pageIndex;

                await LoadCurrentPageAsync(_loadCts.Token); // <-- Token reinreichen
            }
            catch (OperationCanceledException)
            {
                CurrentPageIndex = oldindex;
            }
            catch (Exception ex)
            {
                _errorService.HandleException(ex, "GoToPageAsync", true, false);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public static DataTable ToDataTable(CsvPage page)
        {
            var table = new DataTable();

            foreach (var h in page.Headers)
                table.Columns.Add(h);

            foreach (var r in page.Rows)
            {
                // falls Zeile weniger Spalten hat, auffüllen
                var row = new object[page.Headers.Length];
                for (int i = 0; i < row.Length; i++)
                    row[i] = i < r.Length ? r[i] : "";

                table.Rows.Add(row);
            }

            return table;
        }

        /// <summary>
        /// Hier kommt dein neuer Viewer-Ladecode rein:
        /// - StartZeile berechnen
        /// - PageSize Zeilen lesen
        /// - PageRows aktualisieren
        /// </summary>
        private async Task LoadCurrentPageAsync(CancellationToken token)
        {
            if (ViewerFile == null) return;
            if (string.IsNullOrEmpty(ViewerFile.Separator))
                throw new InvalidOperationException("Das Trennzeichen (Separator) darf nicht null oder leer sein.");
            token.ThrowIfCancellationRequested();

            string path = ViewerFile.FullPath;
            var page = await CsvPageReader.ReadPageAsync(
                filePath: path,
                pageIndex: CurrentPageIndex,
                pageSize: DefaultPageSize,
                separator: ViewerFile.Separator[0],
                hasHeader: true,
                token: token);

            var table = ToDataTable(page);
            var baseIndex = (int)((CurrentPageIndex - 1) * (long)DefaultPageSize) + 1;
            var indexColumn = new DataColumn("#", typeof(int));

            table.Columns.Add(indexColumn);
            indexColumn.SetOrdinal(0);

            for (int i = 0; i < table.Rows.Count; i++)
            {
                table.Rows[i][indexColumn] = baseIndex + i;
            }

            _pageTable = table;
            OnPropertyChanged(nameof(PageView));
        }

    }
}
