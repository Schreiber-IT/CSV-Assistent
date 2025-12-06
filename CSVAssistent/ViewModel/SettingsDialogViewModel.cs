using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using CSVAssistent.Core;
using CSVAssistent.Services;
using CSVAssistent.Services.ErrorLog;
using CSVAssistent.Services.Settings;

namespace CSVAssistent.ViewModel
{
    public class SettingsDialogViewModel : BaseViewModel
    {
        private readonly IErrorService _errorService;
        private readonly IWindowService _windowService;
        private readonly ISettingsService _settingsService;

        public ICommand NavigateSettings1Command { get; }
        public ICommand NavigateSettings2Command { get; }
        public ICommand NavigateAppSettingsCommand { get; }
        public ICommand NavigateErrorListCommand { get; }

        private readonly AppSettingsViewModel _appsettingsViewModel;
        private readonly ErrorListViewModel _errorlistViewModel;
        private BaseViewModel? _currentViewModel;
        public BaseViewModel? CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
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

        // Definitionen
        private string windowState = "SettingsDialogWindowState";

        public SettingsDialogViewModel()
        {
            _windowService = ServiceLocator.WindowService;
            _settingsService = ServiceLocator.SettingsService;
            _errorService = ServiceLocator.ErrorService;

            _appsettingsViewModel = new AppSettingsViewModel();
            _errorlistViewModel = new ErrorListViewModel();

            NavigateAppSettingsCommand = new RelayCommand(_ => NavigateAppSettings());
            NavigateErrorListCommand = new RelayCommand(_ => NavigateToErrorList());

            LoadSettings();
            NavigateAppSettings();
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
                // -
                // --------------------------------------------------------------------------

                // --------------------------------------------------------------------------

            }
            catch (Exception ex)
            {
                _errorService.HandleException(
                    ex,
                    context: "Settings-LoadSettings",
                    showToUser: true,
                    isExpected: false);
            }
        }

        private void NavigateAppSettings()
        {
            try
            {
                CurrentViewModel = _appsettingsViewModel;
            }
            catch (Exception ex)
            {
                _errorService.HandleException(
                    ex,
                    context: "NavigateToSettings3",
                    showToUser: true,
                    isExpected: false);
            }
        }

        private void NavigateToErrorList()
        {
            try
            {
                CurrentViewModel = _errorlistViewModel;
            }
            catch (Exception ex)
            {
                _errorService.HandleException(
                    ex,
                    context: "NavigateToErrorList",
                    showToUser: true,
                    isExpected: false);
            }
        }

        //private void NavigateToSettings1() => CurrentViewModel = _settings1ViewModel;
        //private void NavigateToSettings2() => CurrentViewModel = _settings2ViewModel;
        //private void NavigateToSettings3() => CurrentViewModel = _settings3ViewModel;
        //private void NavigateToErrorList() => CurrentViewModel = _settings4ViewModel;

    }
}
