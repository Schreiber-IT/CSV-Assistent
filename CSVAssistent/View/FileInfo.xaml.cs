using CSVAssistent.Core;
using CSVAssistent.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSVAssistent.View
{
    public partial class FileInfo : BaseWindow
    {
        public FileInfo()
        {
            InitializeComponent();

            DataContext = new FileInfoViewModel();
        }
    }
}
