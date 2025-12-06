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

        private readonly IErrorService _errorService;
        private readonly ISettingsService _settingsService;
        private readonly IWindowService _windowService;

        public ObservableCollection<string> DoubleClickOptions { get; }

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

            LoadDoubleClickSetting();
        }

        private void LoadDoubleClickSetting()
        {
            try
            {
                var setting = _settingsService.GetString(DoubleClickSettingKey, "DateiInfo");
                if (!DoubleClickOptions.Contains(setting))
                    setting = "DateiInfo";

                SelectedDBClick = setting;
            }
            catch (Exception ex)
            {
                _errorService.HandleException(ex, "AppSettings-LoadDoubleClick", true, false);
            }
        }
    }

}
