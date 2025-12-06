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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CSVAssistent.ViewModel;

namespace CSVAssistent.View.Pages
{
    /// <summary>
    /// Interaktionslogik für Settings.xaml
    /// </summary>
    public partial class ErrorList : System.Windows.Controls.UserControl
    {
        public ErrorList()
        {
            InitializeComponent();
            DataContext = new ErrorListViewModel();
        }
    }
}
