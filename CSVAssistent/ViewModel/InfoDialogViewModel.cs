using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CSVAssistent.Services;
using CSVAssistent.Services.ErrorLog;
using CSVAssistent.Services.Settings;

namespace CSVAssistent.ViewModel
{
    public class InfoDialogViewModel
    {
        private readonly IWindowService _windowService;
        private readonly IErrorService _errorService;
        private readonly ISettingsService _settingsService;

        public string LicenseFile { get; set; }
        public string[] _licenseFile { get; set; }

        public InfoDialogViewModel()
        {
            _windowService = ServiceLocator.WindowService;
            _settingsService = ServiceLocator.SettingsService;
            _errorService = ServiceLocator.ErrorService;

            _licenseFile = File.ReadAllLines("LICENSE");
            StringBuilder sb = new StringBuilder();
            foreach (var line in _licenseFile)
            {
                sb.AppendLine(line);
            }
            LicenseFile = sb.ToString();


        }
    }
}
