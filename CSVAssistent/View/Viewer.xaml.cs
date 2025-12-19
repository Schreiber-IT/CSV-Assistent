using CSVAssistent.Core;
using CSVAssistent.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CSVAssistent.View
{
    public partial class Viewer : BaseWindow
    {
        public Viewer()
        {
            InitializeComponent();

            DataContext = new ViewerViewModel();

        }

        private void DataGridViewer_AutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            // Erzwinge Indexer-Binding für DataRowView (DataTable/DataView)
            if (e.Column is DataGridTextColumn textColumn)
            {
                var name = e.PropertyName; // entspricht DataColumn.ColumnName
                textColumn.Binding = new System.Windows.Data.Binding($"[{name}]")
                {
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.Explicit
                };
            }
        }
    }
}
