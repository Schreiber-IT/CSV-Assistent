using CSVAssistent.Core;
using CSVAssistent.Services;
using CSVAssistent.Services.ErrorLog;
using CSVAssistent.Services.Settings;
using CSVAssistent.Services.Theming;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CSVAssistent.ViewModel
{
    public class AppSettingsViewModel : BaseViewModel
    {
        public const string DoubleClickSettingKey = "DoubleClickSelection";
        public const string RowLimitKey = "RowLimit";
        public const string MaxRowKey = "MaxRows";

        private readonly IErrorService _errorService;
        private readonly ISettingsService _settingsService;
        private readonly IWindowService _windowService;
        private readonly IThemeService _themeService;
         
        public ObservableCollection<string> DoubleClickOptions { get; }
        public ObservableCollection<string> RowLimit { get; }
        public ObservableCollection<string> MaxRows { get; }
        public ObservableCollection<string> ThemeOptions { get; }

        private string _selectedDbClick = "DateiInfo";
        public string SelectedDBClick
        {
            get => _selectedDbClick;
            set
            {
                if (SetProperty(ref _selectedDbClick, value))
                {
                    try
                    {
                        _settingsService.SetString(DoubleClickSettingKey, value);
                    }
                    catch (Exception ex)
                    {
                        _errorService.HandleException(ex, "AppSettings-SetDoubleClick", true, false);
                    }
                }
            }
        }
        

        private string _selectedMaxRows = "100_000";
        public string SelectedMaxRows
        {
            get => _selectedMaxRows;
            set
            {
                if (SetProperty(ref _selectedMaxRows, value))
                {
                    try
                    {
                        _settingsService.SetString(MaxRowKey, value);
                    }
                    catch (Exception ex)
                    {
                        _errorService.HandleException(ex, "AppSettings-SelectedMaxRows", true, false);
                    }
                }
            }
        }

        private string _selectedRowLimit = "100_000";
        public string SelectedRowLimit
        {
            get => _selectedRowLimit;
            set
            {
                if (SetProperty(ref _selectedRowLimit, value))
                {
                    try
                    {
                        _settingsService.SetString(RowLimitKey, value);
                    }
                    catch (Exception ex)
                    {
                        _errorService.HandleException(ex, "AppSettings-RowLimit", true, false);
                    }
                }
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
                if (!string.IsNullOrEmpty(ActiveTheme))
                {
                    _themeService.ApplyTheme(ActiveTheme);
                }
                OnPropertyChanged();
            }
        }

        // Neue Eigenschaften für Slider
        private int _menuIconSize;
        public int MenuIconSize
        {
            get => _menuIconSize;
            set
            {
                if (SetProperty(ref _menuIconSize, value))
                {
                    try
                    {
                        _settingsService.SetInt("MenuIconSize", value);
                    }
                    catch (Exception ex)
                    {
                        _errorService.HandleException(ex, "AppSettings-MenuIconSize", true, false);
                    }
                }
            }
        }

        private int _menuTextSize;
        public int MenuTextSize
        {
            get => _menuTextSize;
            set
            {
                if (SetProperty(ref _menuTextSize, value))
                {
                    try
                    {
                        _settingsService.SetInt("MenuTextSize", value);
                    }
                    catch (Exception ex)
                    {
                        _errorService.HandleException(ex, "AppSettings-MenuTextSize", true, false);
                    }
                }
            }
        }

        public AppSettingsViewModel()
        {
            _windowService = ServiceLocator.WindowService;
            _settingsService = ServiceLocator.SettingsService;
            _themeService = ServiceLocator.ThemeService;
            _errorService = ServiceLocator.ErrorService;

            ThemeOptions = new ObservableCollection<string>();
            var theme = ServiceLocator.ThemeService.AvailableThemes;
            for (int i = 0; i < theme.Count; i++)
            {
                ThemeOptions.Add(theme[i].Id);
            }
            ActiveTheme = _themeService.CurrentThemeId;

            DoubleClickOptions = new ObservableCollection<string>
            {
                "Viewer",
                "Viewer_Neu",
                "FileSplit",
                "Zuordnungsliste"
            };

            MaxRows = new ObservableCollection<string>
            {
                "10_000",
                "50_000",
                "100_000",
                "250_000",
                "500_000",
                "1_000_000"
            };

            RowLimit = new ObservableCollection<string>
            {
                "100_000",
                "250_000",
                "500_000",
                "750_000",
                "1_000_000",
                "2_000_000",
                "3_000_000",
                "4_000_000",
                "5_000_000"
            };

            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                var setting = _settingsService.GetString(DoubleClickSettingKey, "Viewer");
                if (!DoubleClickOptions.Contains(setting))
                    setting = "Viewer";
                SelectedDBClick = setting;

                var setting2 = _settingsService.GetString(RowLimitKey, "100_000");
                if (!RowLimit.Contains(setting2))
                    setting2 = "100_000";
                SelectedRowLimit = setting2;

                var setting3 = _settingsService.GetString(MaxRowKey, "100_000");
                if (!MaxRows.Contains(setting3))
                    setting3 = "100_000";
                SelectedMaxRows = setting3;
              

                // Initialwerte für Slider laden
                MenuIconSize = _settingsService.GetInt("MenuIconSize", 20);
                MenuTextSize = _settingsService.GetInt("MenuTextSize", 10);
            }
            catch (Exception ex)
            {
                _errorService.HandleException(ex, "AppSettings-LoadSettings", true, false);
            }
        }
    }
}
