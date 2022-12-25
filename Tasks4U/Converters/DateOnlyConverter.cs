using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Tasks4U.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class DateOnlyConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateOnly date && date != DateOnly.MinValue)
                return date;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            DependencyProperty.UnsetValue;
    }
}
