using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Table = System.Windows.Documents.Table;
using Task = Tasks4U.Models.Task;

namespace Tasks4U.FlowDocumentGenerators
{
    public class TasksListDocumentGenerator
    {
        private const int NumOfColumns = 8;        

        public FlowDocument Generate(IEnumerable<Task>? tasks)
        {
            var flowDocument = new FlowDocument() { ColumnWidth = double.MaxValue, Name="Tasks_List" };

            if (tasks == null)
                return flowDocument;

            var table = GenerateTable(tasks);

            flowDocument.Blocks.Add(table);

            return flowDocument;
        }

        private Table GenerateTable(IEnumerable<Task> tasks)
        {
            var table = new Table()
            {
                CellSpacing = 0,
                Background = Brushes.White,
                TextAlignment = TextAlignment.Center,
            };

            for (int i = 0; i < NumOfColumns; i++)
                table.Columns.Add(new TableColumn());

            table.Columns[1].Width = new GridLength(200);

            table.RowGroups.Add(new TableRowGroup());

            table.RowGroups[0].Rows.Add(GenerateHeaderRow());

            foreach (var task in tasks)
                table.RowGroups[0].Rows.Add(GenerateTaskRow(task));

            return table;
        }

        private TableRow GenerateHeaderRow()
        {
            var row = new TableRow()
            {
                Background = Brushes.Silver,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
            };

            var cellContents = new string[] 
                { "Subject", "Description", "Related To", "Desk", "Task Frequency", "Intermediate Date", "Final Date", "Status" };

            foreach (var content in cellContents)
                row.Cells.Add(GenerateCell(content));

            return row;
        }

        private TableRow GenerateTaskRow(Task task)
        {
            var row = new TableRow();

            var cellContents = new string[] 
            {
                task.Name, 
                DocumentGeneratorUtils.GetTaskDescription(task), 
                task.RelatedTo, 
                task.Desk.ToString(),
                DocumentGeneratorUtils.SplitByCapitalLetters(task.TaskFrequency),
                task.IntermediateDate.ToString(), 
                task.FinalDate.ToString(),
                DocumentGeneratorUtils.SplitByCapitalLetters(task.Status)
            };

            foreach (var content in cellContents)
                row.Cells.Add(GenerateCell(content));

            return row;
        }

        private TableCell GenerateCell(string text)
        {
            var paragraph = new Paragraph(new Run(text));

            return new TableCell(paragraph)
            {
                BorderThickness = new Thickness(1, 0, 0, 1),
                BorderBrush = Brushes.Black
            };
        }       
    }
}
