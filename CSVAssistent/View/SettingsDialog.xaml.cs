using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CSVAssistent.Core;
using CSVAssistent.Services;
using CSVAssistent.ViewModel;
using CSVAssistent.View.Pages;

namespace CSVAssistent.View
{
    /// <summary>
    /// Interaktionslogik für Vorlage.xaml
    /// </summary>
    public partial class SettingsDialog : BaseWindow
    {
        public SettingsDialog()
        {
            InitializeComponent();
        }

        private void BaseWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is SettingsDialogViewModel vm)
            {
                vm.OnWindowClosing();
            }
        }
    }
}
