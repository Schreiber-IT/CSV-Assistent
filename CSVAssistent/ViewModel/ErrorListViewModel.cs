using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CSVAssistent.Core;
using CSVAssistent.Services;
using CSVAssistent.Services.ErrorLog;
using CSVAssistent.Services.Settings;

namespace CSVAssistent.ViewModel
{
    public class ErrorListViewModel : BaseViewModel
    {
        private readonly IErrorService _errorService;
        private readonly ISettingsService _settingsService;
        private readonly IWindowService _windowService;

        public ObservableCollection<ErrorLogEntry> Errors { get; } = new();

        private ErrorLogEntry _selectedError;
        public ErrorLogEntry SelectedError
        {
            get => _selectedError;
            set
            {
                _selectedError = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand RefreshCommand { get; }

        public ErrorListViewModel()
        {
            _windowService = ServiceLocator.WindowService;
            _settingsService = ServiceLocator.SettingsService;
            _errorService = ServiceLocator.ErrorService;

            RefreshCommand = new RelayCommand(_ => LoadErrors());

            LoadErrors();
        }

        private void LoadErrors()
        {
            Errors.Clear();

            var entries = _errorService
                .GetLastErrors(200)
                .OrderByDescending(e => e.Timestamp);

            foreach (var e in entries)
            {
                Errors.Add(e);
            }
        }
    }

}
