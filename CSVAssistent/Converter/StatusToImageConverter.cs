using System;
using System.Globalization;
using System.Windows.Data;

namespace CSVAssistent.Converter
{
    public class StatusToImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var status = 0;
            if (value is int i) status = i;
            else if (value is string s && int.TryParse(s, out var parsed)) status = parsed;

            var uri = status == 1
                ? "pack://application:,,,/Images/dotgreen.png"
                : "pack://application:,,,/Images/dotred.png";

            return uri;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}