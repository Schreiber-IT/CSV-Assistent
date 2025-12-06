using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CSVAssistent.ViewModel;

namespace CSVAssistent.Services
{
    public interface IWindowService
    {
        void Show<TViewModel>(TViewModel viewModel, bool hideOwnerWhileOpen = false) where TViewModel : class;
        bool? ShowDialog<TViewModel>(TViewModel viewModel, bool hideOwnerWhileOpen = false) where TViewModel : class;
    }
}
