using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;
using Tasks4U.FlowDocumentGenerators;

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
                description = GetDescription((string)value);
            }
            catch (XamlParseException)
            {
                try
                {
                    description = GetDescription(((string)value).WithoutBitmapImages());
                }
                catch (XamlParseException)
                {
                    // If we cannot parse the given value as XAML, treat it as plain text
                    description = (string)value;
                }
            }

            using var stringReader = new StringReader(description);
            
            var firstLine = stringReader.ReadLine();

            while (firstLine != null && firstLine.Trim().Length == 0)
                firstLine = stringReader.ReadLine();

            return firstLine ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            DependencyProperty.UnsetValue;

        private string GetDescription(string value)
        {
            var descriptionDocument = (FlowDocument)XamlReader.Parse((string)value);
            return new TextRange(descriptionDocument.ContentStart, descriptionDocument.ContentEnd).Text;
        }
    }
}
