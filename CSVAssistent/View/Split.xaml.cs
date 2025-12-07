using CSVAssistent.Core;
using CSVAssistent.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSVAssistent.View
{
    public partial class Split : BaseWindow
    {
        public Split()
        {
            InitializeComponent();

            DataContext = new SplitViewModel();

        }
    }
}
