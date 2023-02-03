using System;
using System.Globalization;
using System.Windows.Data;
using Tasks4U.Models;

namespace Tasks4U.Converters
{
    public class DateAndFrequencyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is DateOnly date && date != DateOnly.MinValue && values[1] is Frequency frequency)
            {
                var dateFormat = frequency switch
                {
                    Frequency.EveryWeek => "dddd", // Week Day, e.g., Sunday
                    Frequency.EveryMonth => "dd", // Day in month, e.g., 01
                    Frequency.EveryYear => "MMMM dd", // Full month name followed by day in month, e.g., January 1
                    _ => "dd/MM/yyyy" // Full date, e.g., 01/01/2023
                };

                return date.ToString(dateFormat);
            }

            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
