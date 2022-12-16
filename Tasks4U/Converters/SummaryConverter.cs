using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.IO;

namespace Tasks4U.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class SummaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            using (var stringReader = new StringReader((string)value))
            {
                var firstLine = stringReader.ReadLine();

                if (firstLine == null)
                    return string.Empty;

                var maxLen = parameter is int n ? n : 10;

                if (maxLen > firstLine.Length)
                    return firstLine;

                return firstLine.Substring(0, maxLen);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            DependencyProperty.UnsetValue;
    }
}
