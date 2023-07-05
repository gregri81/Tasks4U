using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using Tasks4U.Models;

namespace Tasks4U.FlowDocumentGenerators
{
    public class TasksListWorksheetGenerator
    {
        public IXLWorksheet Generate(XLWorkbook excelWorkbook, IEnumerable<Task>? tasks)
        {
            var worksheet = excelWorkbook.Worksheets.Add("Tasks");

            if (tasks == null)
                return worksheet;

            AddHeaderRow(worksheet);

            var row = 1;

            foreach (var task in tasks)
                AddTaskRow(worksheet, task, ++row);

            SetAsDropDown(worksheet, typeof(Desk), 4);
            SetAsDropDown(worksheet, typeof(TaskStatus), 8);

            worksheet.Cells("A1:H1").Style.Fill.BackgroundColor = XLColor.Green;
            worksheet.Columns().AdjustToContents();

            return worksheet;
        }

        private void AddHeaderRow(IXLWorksheet worksheet)
        {
           var headers = new string[]
           {
                "Subject", "Description", "RelatedTo", "Desk", "Type", "Frequency", "Intermediate Date", "Final Date", "Status"
           };

            for (int i = 0; i < headers.Length; i++)
                worksheet.Cell(1, i + 1).SetValue(headers[i]);
        }

        private void AddTaskRow(IXLWorksheet worksheet, Task task, int row)
        {
            worksheet.Cell(row, 1).SetValue(task.Name);
            worksheet.Cell(row, 2).SetValue(Utils.GetTaskDescription(task));
            worksheet.Cell(row, 3).SetValue(task.RelatedTo);            
            worksheet.Cell(row, 4).SetValue(task.Desk.ToString());
            worksheet.Cell(row, 5).SetValue(task.TaskType.ToString());
            worksheet.Cell(row, 6).SetValue(Utils.SplitByCapitalLetters(task.TaskFrequency));

            string dateFormat = GetDateFormat(task.TaskFrequency);

            if (task.IntermediateDate > DateOnly.MinValue)
            {
                worksheet.Cell(row, 7).Style.DateFormat.SetFormat(dateFormat);
                worksheet.Cell(row, 7).SetValue(task.IntermediateDate.ToDateTime(TimeOnly.MinValue));
            }

            worksheet.Cell(row, 8).Style.DateFormat.SetFormat(dateFormat);
            worksheet.Cell(row, 8).SetValue(task.FinalDate.ToDateTime(TimeOnly.MinValue));

            worksheet.Cell(row, 9).SetValue(Utils.SplitByCapitalLetters(task.Status));
        }

        private void SetAsDropDown(IXLWorksheet worksheet, Type enumType, int column)
        {
            var names = Enum.GetNames(enumType).Select(name => Utils.SplitByCapitalLetters(name));

            worksheet.Column(column).CreateDataValidation().List('"' + string.Join(',', names) + '"');
        }

        private string GetDateFormat(Frequency taskFrequency)
        {
            return taskFrequency switch
            {
                Frequency.EveryWeek => "dddd",
                Frequency.EveryMonth => "dd",
                Frequency.EveryYear => "MMMM dd",
                _ => "dd/mm/yy"
            };
        }
    }
}
