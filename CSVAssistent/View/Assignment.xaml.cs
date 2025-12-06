using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using CSVAssistent.Core;
using CSVAssistent.Services;
using CSVAssistent.ViewModel;

namespace CSVAssistent.View
{
    public partial class Assignment : BaseWindow
    {
        public Assignment()
        {
            InitializeComponent();
            DataContext = new AssignmentViewModel();
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (DataContext is AssignmentViewModel vm && vm.AddMappingCommand.CanExecute(null))
                    vm.AddMappingCommand.Execute(null);
                e.Handled = true;
            }
        }
}
}
