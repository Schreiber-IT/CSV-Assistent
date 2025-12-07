using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;
using CSVAssistent.Services;
using CSVAssistent.Services.ErrorLog;
using CSVAssistent.Services.Settings;
using CSVAssistent.Services.Theming;

namespace CSVAssistent
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : System.Windows.Application
    {
        public LiteDbLogService? LiteDbLogService { get; private set; }
        public ILogService? LogService { get; private set; }
        public IUserNotificationService? NotificationService { get; private set; }
        public IErrorService? ErrorService { get; private set; }
        public LiteDbSettingsService? SettingsService { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Services initialisieren
            LiteDbLogService = new LiteDbLogService("Filename=errorlogs.db;Connection=shared;");
            LogService = LiteDbLogService;
            NotificationService = new UserNotificationService();

            ServiceLocator.ErrorService = new ErrorService(LogService, NotificationService);
            ServiceLocator.WindowService = new WindowService();
            ServiceLocator.SettingsService = new LiteDbSettingsService("Filename=settings.db;Connection=shared;");
            ServiceLocator.ThemeService = new ThemeService();
            ServiceLocator.ColumnAssignmentService = new ColumnAssignmentService();

            // Globale Handler setzen
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;


            var themeId = "Dark"; // Standard-Theme
            //var themeService = new ThemeService();
            // themeService.ApplyTheme(themeService.CurrentThemeId);
            ServiceLocator.ThemeService.ApplyTheme(themeId);

            var mainWindow = new View.MainWindow
            {
                DataContext = new ViewModel.MainViewModel()
            };
            mainWindow.Show();
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ErrorService?.HandleException(e.Exception, "Unerwarteter Fehler im UI-Thread", showToUser: true);
            e.Handled = true; // damit die App nicht abstürzt
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            ErrorService?.HandleException(e.Exception, "Unerwarteter Fehler in Hintergrund-Task", showToUser: true);
            e.SetObserved();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (LogService is IDisposable d)
                d.Dispose();

            base.OnExit(e);
        }
    }


}


