using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using CSVAssistent.ViewModel;

namespace CSVAssistent.Services
{
    public class WindowService : IWindowService
    {
        private readonly Dictionary<Type, WindowRegistration> _registrations = new();

        public WindowService()
        {
            // Produktive Registrierungen
            Register<InfoDialogViewModel, View.InfoDialog>(singleInstance: true);
            Register<SettingsDialogViewModel, View.SettingsDialog>(singleInstance: true);
            Register<HelpViewModel, View.Help>(singleInstance: true);
            Register<FileInfoViewModel, View.FileInfo>(singleInstance: true);
            Register<AssignmentViewModel, View.Assignment>(singleInstance: true);
            Register<SplitViewModel, View.Split>(singleInstance: true);
            Register<ViewerViewModel, View.Viewer>(singleInstance: true);
        }

        public void Register<TViewModel, TWindow>(bool singleInstance)
            where TViewModel : class
            where TWindow : Window
        {
            _registrations[typeof(TViewModel)] =
                new WindowRegistration(typeof(TViewModel), typeof(TWindow), singleInstance);
        }

        public void Show<TViewModel>(TViewModel viewModel, bool hideOwnerWhileOpen = false)
            where TViewModel : class
        {
            var window = GetOrCreateWindow(viewModel, hideOwnerWhileOpen);
            window.Show();
        }

        public bool? ShowDialog<TViewModel>(TViewModel viewModel, bool hideOwnerWhileOpen = false)
            where TViewModel : class
        {
            var window = GetOrCreateWindow(viewModel, hideOwnerWhileOpen);
            return window.ShowDialog();
        }

        private Window GetOrCreateWindow<TViewModel>(TViewModel viewModel, bool hideOwnerWhileOpen)
            where TViewModel : class
        {
            var vmType = viewModel.GetType();

            if (!_registrations.TryGetValue(vmType, out var registration))
                throw new InvalidOperationException($"Kein Window für ViewModel {vmType.Name} registriert.");

            // SINGLE INSTANCE: existierendes Fenster suchen
            if (registration.SingleInstance)
            {
                var existingWindow = System.Windows.Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.GetType() == registration.WindowType);

                if (existingWindow != null)
                {
                    // ViewModel ggf. aktualisieren
                    if (existingWindow.DataContext != viewModel)
                        existingWindow.DataContext = viewModel;

                    existingWindow.Activate();
                    return existingWindow;
                }
            }

            // Neues Fenster erstellen
            var window = (Window)Activator.CreateInstance(registration.WindowType)!;
            window.DataContext = viewModel;

            var owner = System.Windows.Application.Current?.MainWindow;
            if (owner != null && window != owner)
            {
                window.Owner = owner;

                if (hideOwnerWhileOpen)
                {
                    owner.Hide();
                    window.Closed += (_, __) =>
                    {
                        // wenn Owner noch existiert und versteckt ist -> wieder anzeigen
                        if (owner.Visibility == Visibility.Hidden)
                            owner.Show();
                    };
                }
            }

            return window;
        }
    }
}
