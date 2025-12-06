using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;
using CSVAssistent.Core;
using CSVAssistent.Services;
using CSVAssistent.Services.ErrorLog;
using CSVAssistent.Services.Settings;

namespace CSVAssistent.ViewModel
{
    public class HelpViewModel : BaseViewModel
    {
        private readonly IWindowService _windowService;
        private readonly IErrorService _errorService;
        private readonly ISettingsService _settingsService;

        private BaseViewModel? _currentViewModel;
        public BaseViewModel? CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }
        private string _helpFile = string.Empty;
        public string HelpFile
        {
            get => _helpFile;
            set => SetProperty(ref _helpFile, value);
        }

        public ICommand ShowDEHelpCommand { get; }
        public ICommand ShowENHelpCommand { get; }
        


        public HelpViewModel()
        {
            _windowService = ServiceLocator.WindowService;
            _settingsService = ServiceLocator.SettingsService;
            _errorService = ServiceLocator.ErrorService;

            ShowDEHelpCommand = new RelayCommand(_ => NavigateToDEHelp());
            ShowENHelpCommand = new RelayCommand(_ => NavigateToENHelp());

            NavigateToDEHelp();
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
