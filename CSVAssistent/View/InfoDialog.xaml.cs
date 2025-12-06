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

namespace CSVAssistent.View
{
    public partial class InfoDialog : BaseWindow
    {
        public InfoDialog()
        {
            InitializeComponent();

            DataContext = new InfoDialogViewModel();

        }
    }
}
