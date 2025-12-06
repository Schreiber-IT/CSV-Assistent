using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace CSVAssistent.Core
{
    public class BaseWindow : Window
    {
        public BaseWindow()
        {
            // Standard-Fenster-Einstellungen
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.CanResizeWithGrip;
            // Background, etc. kommen über Styles/Themes

            // Optional: CommandBindings für Min/Max/Close
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand,
                (_, __) => SystemCommands.CloseWindow(this)));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand,
                (_, __) => SystemCommands.MinimizeWindow(this)));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand,
                (_, __) => SystemCommands.MaximizeWindow(this)));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand,
                (_, __) => SystemCommands.RestoreWindow(this)));
        }

        public void Window_StateChanged(object? sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                Topmost = false; // garantiert
        }

        public void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
            {
                this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
        }

        public void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Ermöglicht das Ziehen des Fensters
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
