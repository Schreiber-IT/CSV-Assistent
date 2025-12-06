using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CSVAssistent.Core.Behaviors
{
    public static class FileDropBehavior
    {
        public static readonly DependencyProperty DropCommandProperty =
            DependencyProperty.RegisterAttached(
                "DropCommand",
                typeof(ICommand),
                typeof(FileDropBehavior),
                new PropertyMetadata(null, OnDropCommandChanged));

        public static void SetDropCommand(DependencyObject element, ICommand value)
            => element.SetValue(DropCommandProperty, value);

        public static ICommand? GetDropCommand(DependencyObject element)
            => (ICommand?)element.GetValue(DropCommandProperty);

        private static void OnDropCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not UIElement ui) return;

            if (e.NewValue != null)
            {
                ui.AllowDrop = true;
                ui.PreviewDragOver += OnPreviewDragOver;
                ui.Drop += OnDrop;
            }
            else
            {
                ui.PreviewDragOver -= OnPreviewDragOver;
                ui.Drop -= OnDrop;
            }
        }

        private static void OnPreviewDragOver(object sender, System.Windows.DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                e.Effects = System.Windows.DragDropEffects.None;
                e.Handled = true;
                return;
            }
            e.Effects = System.Windows.DragDropEffects.Copy;
            e.Handled = true;
        }

        private static void OnDrop(object sender, System.Windows.DragEventArgs e)
        {
            var command = GetDropCommand((DependencyObject)sender);
            if (command == null) return;

            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                var paths = ((string[])e.Data.GetData(System.Windows.DataFormats.FileDrop))
                    .Where(p => p.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (paths.Count == 0) return;
                if (command.CanExecute(paths))
                    command.Execute(paths);
            }
        }
    }
}