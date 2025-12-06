using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace CSVAssistent.Core.Behaviors
{
    public static class GridViewColumnGenerator
    {
        public static readonly DependencyProperty ColumnsSourceProperty =
            DependencyProperty.RegisterAttached(
                "ColumnsSource",
                typeof(IEnumerable),
                typeof(GridViewColumnGenerator),
                new PropertyMetadata(null, OnColumnsSourceChanged));

        public static void SetColumnsSource(DependencyObject element, IEnumerable value)
            => element.SetValue(ColumnsSourceProperty, value);

        public static IEnumerable GetColumnsSource(DependencyObject element)
            => (IEnumerable)element.GetValue(ColumnsSourceProperty);

        private static void OnColumnsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not GridView gv) return;

            if (e.OldValue is INotifyCollectionChanged oldObs)
                oldObs.CollectionChanged -= (_, __) => RebuildColumns(gv);

            if (e.NewValue is INotifyCollectionChanged newObs)
                newObs.CollectionChanged += (_, __) => RebuildColumns(gv);

            RebuildColumns(gv);
        }

        private static void RebuildColumns(GridView gv)
        {
            var cols = GetColumnsSource(gv);
            gv.Columns.Clear();
            if (cols == null) return;

            foreach (var item in cols)
            {
                var colName = item?.ToString() ?? "";
                gv.Columns.Add(new GridViewColumn
                {
                    Header = new GridViewColumnHeader
                    {
                        Content = colName,
                        Tag = colName
                    },
                    DisplayMemberBinding = new System.Windows.Data.Binding($"[{colName}]")
                });
            }
        }
    }
}