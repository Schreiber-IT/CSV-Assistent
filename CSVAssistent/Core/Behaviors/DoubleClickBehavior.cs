using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CSVAssistent.Core.Behaviors
{
    public static class DoubleClickBehavior
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(DoubleClickBehavior),
                new PropertyMetadata(null, OnCommandChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "CommandParameter",
                typeof(object),
                typeof(DoubleClickBehavior),
                new PropertyMetadata(null));

        public static void SetCommand(DependencyObject element, ICommand value)
            => element.SetValue(CommandProperty, value);

        public static ICommand GetCommand(DependencyObject element)
            => (ICommand)element.GetValue(CommandProperty);

        public static void SetCommandParameter(DependencyObject element, object value)
            => element.SetValue(CommandParameterProperty, value);

        public static object GetCommandParameter(DependencyObject element)
            => element.GetValue(CommandParameterProperty);

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is System.Windows.Controls.ListBox listBox)
            {
                if (e.NewValue != null)
                    listBox.MouseDoubleClick += ListBox_MouseDoubleClick;
                else
                    listBox.MouseDoubleClick -= ListBox_MouseDoubleClick;
            }
        }

        private static void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not System.Windows.Controls.ListBox listBox) return;

            // ermittele das angeklickte Item
            var container = GetAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);
            var item = container?.DataContext ?? listBox.SelectedItem;
            var command = GetCommand(listBox);
            var parameter = GetCommandParameter(listBox) ?? item;

            if (command?.CanExecute(parameter) == true)
                command.Execute(parameter);
        }

        private static T? GetAncestor<T>(DependencyObject o) where T : DependencyObject
        {
            while (o != null)
            {
                if (o is T t) return t;
                o = VisualTreeHelper.GetParent(o);
            }
            return null;
        }
    }
}