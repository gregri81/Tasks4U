using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Tasks4U.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class FirstLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            string description;

            try
            {
                var descriptionDocument = (FlowDocument)XamlReader.Parse((string)value);
                description = new TextRange(descriptionDocument.ContentStart, descriptionDocument.ContentEnd).Text;
            }
            catch (XamlParseException)
            {
                // If we cannot parse the given value as XAML, treat it as plain text
                description = (string)value;
            }

            using var stringReader = new StringReader(description);
            var firstLine = stringReader.ReadLine();
            return firstLine ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            DependencyProperty.UnsetValue;
    }
}
