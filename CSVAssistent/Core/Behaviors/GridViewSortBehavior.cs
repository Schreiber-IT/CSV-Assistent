using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CSVAssistent.Core.Behaviors
{
    public static class GridViewSortBehavior
    {
        public static readonly DependencyProperty SortCommandProperty = DependencyProperty.RegisterAttached(
            "SortCommand",
            typeof(ICommand),
            typeof(GridViewSortBehavior),
            new PropertyMetadata(null, OnSortCommandChanged));

        private static readonly RoutedEventHandler HeaderClickHandler = OnColumnHeaderClick;

        public static void SetSortCommand(DependencyObject element, ICommand value) => element.SetValue(SortCommandProperty, value);

        public static ICommand GetSortCommand(DependencyObject element) => (ICommand)element.GetValue(SortCommandProperty);

        private static void OnSortCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not System.Windows.Controls.ListView listView)
            {
                return;
            }

            listView.RemoveHandler(GridViewColumnHeader.ClickEvent, HeaderClickHandler);

            if (e.NewValue != null)
            {
                listView.AddHandler(GridViewColumnHeader.ClickEvent, HeaderClickHandler);
            }
        }

        private static void OnColumnHeaderClick(object sender, RoutedEventArgs e)
        {
            if (sender is not System.Windows.Controls.ListView listView)
            {
                return;
            }

            var command = GetSortCommand(listView);
            if (command == null)
            {
                return;
            }

            if (e.OriginalSource is not GridViewColumnHeader header)
            {
                return;
            }

            var columnName = header.Tag?.ToString() ?? header.Content?.ToString();
            if (string.IsNullOrWhiteSpace(columnName))
            {
                return;
            }

            if (command.CanExecute(columnName))
            {
                command.Execute(columnName);
            }
        }
    }
}
