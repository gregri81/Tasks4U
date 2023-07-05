using System.Windows;
using Tasks4U.Models;
using Tasks4U.Services;
using Tasks4U.ViewModels;
using Tasks4U.FlowDocumentGenerators;
using TaskStatus = Tasks4U.Models.TaskStatus;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Task4UTests
{
    [TestClass]
    public class TasksViewModelTests
    {
        TasksViewModel _tasksViewModel = CreateTasksViewModel();

        [TestInitialize]
        public void DeleteTasks() => _tasksViewModel = CreateTasksViewModel();

        [TestMethod]
        public void NoTasksIfModelIsEmpty() =>
            Assert.AreEqual(0, _tasksViewModel.Tasks.Count());

        [STATestMethod]
        public void TasksAddedEditedAndRemoved()
        {
            _tasksViewModel.Tasks.Clear();

            // Add first task
            var taskViewModel1 = new TaskViewModel
            {
                Name = "task1",
                TaskFrequency = Frequency.EveryWeek,
                Status = TaskStatus.InProgress,
                Desk = Desk.USA,
                Description = "description",
                IntermediateDate = DateOnly.MaxValue,
                FinalDate = DateOnly.MaxValue,
                RelatedTo = "related",
                NameDirection = FlowDirection.LeftToRight,
                DescriptionDirection = FlowDirection.RightToLeft,
                RelatedToDirection = FlowDirection.RightToLeft,
                TaskType = TaskType.Research
            };

            _tasksViewModel.NewTaskViewModel = taskViewModel1;
            
            var document = new FlowDocument();
            document.Blocks.Clear();
            document.Blocks.Add(new Paragraph(new Run(taskViewModel1.Description)));

            _tasksViewModel.AddTaskCommand.Execute(document);
            
            Assert.AreEqual(1, _tasksViewModel.Tasks.Count());
            Assert.AreEqual("task1", _tasksViewModel.Tasks.First().Name);

            // Add second task
            var taskViewModel2 = new TaskViewModel { Name = "task2" };

            _tasksViewModel.NewTaskViewModel = taskViewModel2;
            _tasksViewModel.AddTaskCommand.Execute(new FlowDocument());

            Assert.AreEqual(2, _tasksViewModel.Tasks.Count());
            Assert.AreEqual("task2", _tasksViewModel.Tasks.Skip(1).First().Name);

            // Select first task
            _tasksViewModel.Tasks.First().IsSelected = true;

            // Edit first task
            _tasksViewModel.EditSelectedTaskCommand.Execute(null);
            var editedTaskViewModel = _tasksViewModel.NewTaskViewModel;

            var descriptionDocument = (FlowDocument)XamlReader.Parse(editedTaskViewModel.Description);
            var description = new TextRange(descriptionDocument.ContentStart, descriptionDocument.ContentEnd).Text.Trim();

            Assert.AreEqual(taskViewModel1.Name, editedTaskViewModel.Name);
            Assert.AreEqual(taskViewModel1.TaskFrequency, editedTaskViewModel.TaskFrequency);
            Assert.AreEqual(taskViewModel1.Status, editedTaskViewModel.Status);
            Assert.AreEqual(taskViewModel1.Desk, editedTaskViewModel.Desk);
            Assert.AreEqual(taskViewModel1.TaskType, editedTaskViewModel.TaskType);
            Assert.AreEqual(taskViewModel1.Description, description);
            Assert.AreEqual(taskViewModel1.IntermediateDate, editedTaskViewModel.IntermediateDate);
            Assert.AreEqual(taskViewModel1.FinalDate, editedTaskViewModel.FinalDate);
            Assert.AreEqual(taskViewModel1.RelatedTo, editedTaskViewModel.RelatedTo);
            Assert.AreEqual(taskViewModel1.NameDirection, editedTaskViewModel.NameDirection);
            Assert.AreEqual(taskViewModel1.RelatedToDirection, editedTaskViewModel.RelatedToDirection);
            Assert.AreEqual(taskViewModel1.DescriptionDirection, editedTaskViewModel.DescriptionDirection);

            // Remove first task
            _tasksViewModel.RemoveSelectedTasksCommand.Execute(null);

            Assert.AreEqual(1, _tasksViewModel.Tasks.Count());
            Assert.AreEqual("task2", _tasksViewModel.Tasks.First().Name);
        }

        [TestMethod]
        public void ShowNewTaskAndShowTasksCommandsTest()
        {
            bool isNewTaskVisible = false, isTasksListVisible = true;

            _tasksViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_tasksViewModel.IsNewTaskVisible))
                    isNewTaskVisible = _tasksViewModel.IsNewTaskVisible;
                else if (e.PropertyName == nameof(_tasksViewModel.IsTasksListVisible))
                    isTasksListVisible = _tasksViewModel.IsTasksListVisible;
            };
                
            _tasksViewModel.ShowNewTaskCommand.Execute(null);

            Assert.IsTrue(isNewTaskVisible, "New task is not visible after executing ShowNewTaskCommand");
            Assert.IsFalse(isTasksListVisible, "Tasks list is visible after executing ShowNewTaskCommand");

            _tasksViewModel.ShowTasksCommand.Execute(null);

            Assert.IsFalse(isNewTaskVisible, "New task is not visible after executing ShowNewTaskCommand");
            Assert.IsTrue(isTasksListVisible, "Tasks list is visible after executing ShowNewTaskCommand");
        }

        class MockMessageBoxService : IMessageBoxService
        {
            public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
            {
                return MessageBoxResult.Yes;
            }
        }

        private static TasksViewModel CreateTasksViewModel()
        {
            var tasksContext = new TasksContext("Data Source=test.db");
            tasksContext.Tasks.RemoveRange(tasksContext.Tasks);
            tasksContext.SaveChanges();

            return new TasksViewModel(tasksContext, 
                                      new MockMessageBoxService(), 
                                      new TasksListFlowDocumentGenerator(),
                                      new TaskFlowDocumentGenerator(),
                                      new TasksListWorksheetGenerator(),
                                      new PdfService());
        }
    }
}
