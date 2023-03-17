using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Tasks4U.Converters
{
    public class StringToFlowDocumentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (FlowDocument)XamlReader.Parse((string)value);
            }
            catch (XamlParseException)
            {
                // If we cannot parse the given value as XAML, treat it as plain text
                var paragraph = new Paragraph(new Run((string)value));
                return new FlowDocument(paragraph);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
