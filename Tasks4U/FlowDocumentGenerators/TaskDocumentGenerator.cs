using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using Tasks4U.Models;
using Tasks4U.ViewModels;

namespace Tasks4U.FlowDocumentGenerators
{
    public class TaskDocumentGenerator
    {
        public FlowDocument Generate(TaskViewModel task, FlowDocument descriptionDocument)
        {
            var flowDocument = new FlowDocument() { ColumnWidth = double.MaxValue, Name = "Task" };

            AddField(flowDocument, "Subject", task.Name);
            AddField(flowDocument, "Related To", task.RelatedTo);
            AddField(flowDocument, "Desk", DocumentGeneratorUtils.SplitByCapitalLetters(task.Desk));
            AddField(flowDocument, "Frequency", DocumentGeneratorUtils.SplitByCapitalLetters(task.TaskFrequency));
            AddField(flowDocument, "Intermediate Date", task.IntermediateDate.ToString());
            AddField(flowDocument, "Final Date", task.FinalDate.ToString());

            flowDocument.Blocks.Add(new Paragraph(new LineBreak()));

            AddDescription(flowDocument, descriptionDocument);

            return flowDocument;
        }

        private void AddField(FlowDocument flowDocument, string fieldName, string fieldValue) 
        {
            var field = GenerateField(fieldName, fieldValue);
            flowDocument.Blocks.Add(field);
        }

        private Paragraph GenerateField(string fieldName, string fieldValue)
        {
            var fieldNameRun = new Run(fieldName + ":");
            var fieldValueRun = new Run(" " + fieldValue);

            var paragraph = new Paragraph(new Underline(fieldNameRun));
            paragraph.Inlines.Add(fieldValueRun);

            return paragraph;
        }

        private void AddDescription(FlowDocument flowDocument, FlowDocument descriptionDocument)
        {
            using (var stream = new MemoryStream())
            {
                var from = new TextRange(descriptionDocument.ContentStart, descriptionDocument.ContentEnd);
                XamlWriter.Save(from, stream);
                from.Save(stream, DataFormats.XamlPackage);
                
                var to = new TextRange(flowDocument.ContentEnd, flowDocument.ContentEnd);
                to.Load(stream, DataFormats.XamlPackage);
            }
        }
    }
}
