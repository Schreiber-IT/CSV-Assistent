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

        private readonly IErrorService _errorService;
        private readonly ISettingsService _settingsService;
        private readonly IWindowService _windowService;
        private readonly IThemeService _themeService;

        public ObservableCollection<string> DoubleClickOptions { get; }
        public ObservableCollection<string> RowLimit { get; }
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
                "DateiInfo",
                "Zuordnungsliste"
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
                var setting = _settingsService.GetString(DoubleClickSettingKey, "DateiInfo");
                if (!DoubleClickOptions.Contains(setting))
                    setting = "DateiInfo";
                SelectedDBClick = setting;

                var setting2 = _settingsService.GetString(RowLimitKey, "100_000");
                if (!RowLimit.Contains(setting2))
                    setting2 = "100_000";
                SelectedRowLimit = setting2;
            }
            catch (Exception ex)
            {
                _errorService.HandleException(ex, "AppSettings-LoadSettings", true, false);
            }
        }
    }

}
