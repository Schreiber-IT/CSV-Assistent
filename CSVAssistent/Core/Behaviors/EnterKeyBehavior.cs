using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CSVAssistent.Core.Behaviors
{
    public static class EnterKeyBehavior
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(EnterKeyBehavior),
                new PropertyMetadata(null, OnCommandChanged));

        public static void SetCommand(DependencyObject element, ICommand value)
            => element.SetValue(CommandProperty, value);

        public static ICommand? GetCommand(DependencyObject element)
            => (ICommand?)element.GetValue(CommandProperty);

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "CommandParameter",
                typeof(object),
                typeof(EnterKeyBehavior),
                new PropertyMetadata(null));

        public static void SetCommandParameter(DependencyObject element, object value)
            => element.SetValue(CommandParameterProperty, value);

        public static object? GetCommandParameter(DependencyObject element)
            => element.GetValue(CommandParameterProperty);

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is System.Windows.Controls.TextBox tb)
            {
                if (e.NewValue != null)
                    tb.PreviewKeyDown += OnPreviewKeyDown;
                else
                    tb.PreviewKeyDown -= OnPreviewKeyDown;
            }
        }

        private static void OnPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.Enter && e.Key != Key.Return) return;
            var tb = (System.Windows.Controls.TextBox)sender;
            var cmd = GetCommand(tb);
            var param = GetCommandParameter(tb);
            System.Windows.MessageBox.Show(cmd.ToString() + "\n" + param.ToString());
            if (cmd?.CanExecute(param) == true)
            {
                cmd.Execute(param);
                e.Handled = true;
            }
        }
    }
}