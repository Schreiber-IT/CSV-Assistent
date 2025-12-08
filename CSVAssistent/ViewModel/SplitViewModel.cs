using CSVAssistent.Core;
using CSVAssistent.Models;
using CSVAssistent.Services;
using CSVAssistent.Services.ErrorLog;
using CSVAssistent.Services.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace CSVAssistent.ViewModel
{
    public class SplitViewModel : BaseViewModel
    {
        private readonly IWindowService _windowService;
        private readonly IErrorService _errorService;
        private readonly ISettingsService _settingsService;

        public int FileCount { get; set; }
        public string RowlimitString { get; set; }

        private FileEntry? _splitFile;
        public FileEntry? SplitFile
        {
            get => _splitFile;
            set
            {
                if (_splitFile == value) return;
                _splitFile = value;
                OnPropertyChanged();
            }
        }

        private string _helpFile = string.Empty;
        public string HelpFile
        {
            get => _helpFile;
            set => SetProperty(ref _helpFile, value);
        }

        public ICommand ShowDEHelpCommand { get; }
        public ICommand ShowENHelpCommand { get; }
        


        public SplitViewModel()
        {
            _windowService = ServiceLocator.WindowService;
            _settingsService = ServiceLocator.SettingsService;
            _errorService = ServiceLocator.ErrorService;


            ShowDEHelpCommand = new RelayCommand(_ => NavigateToDEHelp());
            ShowENHelpCommand = new RelayCommand(_ => NavigateToENHelp());

            NavigateToDEHelp();
        }

        public void Load(FileEntry file)
        {
            SplitFile = file;
            var rowlimitString = _settingsService.GetString(AppSettingsViewModel.RowLimitKey, "100_000");
            if (!int.TryParse(rowlimitString.Replace("_", ""), out var rowlimit))
            {
                rowlimit = 100000;
            }
            RowlimitString = rowlimit.ToString("N0", new CultureInfo("de-DE"));
            FileCount = (int)SplitFile.Lines / rowlimit;

        }

        private void NavigateToDEHelp()
        {
            try
            {
                var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documentation", "Readme_de.html");
                var uri = new Uri(fullPath);
                HelpFile = uri.AbsoluteUri + "?r=" + Guid.NewGuid().ToString("N"); // erzwingt Reload
            }
            catch (Exception ex)
            {
                _errorService.HandleException(
                    ex,
                    context: "NavigateToHome",
                    showToUser: true,
                    isExpected: false);
            }
        }

        private void NavigateToENHelp()
        {
            try
            {
                var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documentation", "Readme_en.html");
                var uri = new Uri(fullPath);
                HelpFile = uri.AbsoluteUri + "?r=" + Guid.NewGuid().ToString("N"); // erzwingt Reload
            }
            catch (Exception ex)
            {
                _errorService.HandleException(
                    ex,
                    context: "NavigateToHome",
                    showToUser: true,
                    isExpected: false);
            }
        }
    }
}
