using System.Globalization;
using System.Windows.Documents;
using System.Windows.Markup;
using Tasks4U.Converters;
using Tasks4U.Models;

namespace Tasks4U.FlowDocumentGenerators
{
    static class DocumentGeneratorUtils
    {
        private static readonly FirstLineConverter _firstLineConverter = new FirstLineConverter();
        private static readonly SplitByCapitalLettersConverter splitByCapitalLettersConverter = new SplitByCapitalLettersConverter();

        public static string SplitByCapitalLetters(object text) => 
            SplitByCapitalLetters(text?.ToString() ?? string.Empty);

        public static string SplitByCapitalLetters(string text) =>
           (string)splitByCapitalLettersConverter.Convert(text, typeof(string), string.Empty, CultureInfo.InvariantCulture);

        public static string GetTaskDescription(Task task) =>
            (string)_firstLineConverter.Convert(task.Description, typeof(string), null, CultureInfo.InvariantCulture);
    }
}
