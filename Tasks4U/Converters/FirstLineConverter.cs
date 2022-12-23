using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.IO;

namespace Tasks4U.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class FirstLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            using var stringReader = new StringReader((string)value);
            var firstLine = stringReader.ReadLine();
            return firstLine ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            DependencyProperty.UnsetValue;
    }
}
