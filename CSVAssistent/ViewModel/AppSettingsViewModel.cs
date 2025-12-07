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
    public class AppSettingsViewModel : BaseViewModel
    {
        public const string DoubleClickSettingKey = "DoubleClickSelection";
        public const string RowLimitKey = "RowLimit";

        private readonly IErrorService _errorService;
        private readonly ISettingsService _settingsService;
        private readonly IWindowService _windowService;

        public ObservableCollection<string> DoubleClickOptions { get; }
        public ObservableCollection<string> RowLimit { get; }

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

        public AppSettingsViewModel()
        {
            _windowService = ServiceLocator.WindowService;
            _settingsService = ServiceLocator.SettingsService;
            _errorService = ServiceLocator.ErrorService;

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
