using System;
using System.Collections.Generic;
using System.Text;
using CSVAssistent.Services.ErrorLog;
using CSVAssistent.Services.Settings;
using CSVAssistent.Services.Dialog;
using CSVAssistent.Services.Theming;

namespace CSVAssistent.Services
{
    public static class ServiceLocator
    {
        public static IErrorService ErrorService { get; set; } = null!;
        public static IWindowService WindowService { get; set; } = null!;
        public static ISettingsService SettingsService { get; set; } = null!;
        public static IFileDialogService FileDialogService { get; set; } = new FileDialogService();

        public static IThemeService ThemeService { get; set; } = null!;
        public static IColumnAssignmentService ColumnAssignmentService { get; set; } = null!;
    }
}
