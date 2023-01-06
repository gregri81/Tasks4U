using System.Windows;
using Tasks4U.Models;
using Tasks4U.Services;
using Tasks4U.ViewModels;
using Task = Tasks4U.Models.Task;
using TaskStatus = Tasks4U.Models.TaskStatus;

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
            Assert.AreEqual(_tasksViewModel.Tasks.Count(), 0);

        [TestMethod]
        public void TasksAddedEditedAndRemoved()
        {
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
                RelatedTo = "related"
            };

            _tasksViewModel.NewTaskViewModel = taskViewModel1;            
            _tasksViewModel.AddTaskCommand.Execute(null);
            
            Assert.AreEqual(_tasksViewModel.Tasks.Count(), 1);
            Assert.AreEqual("task1", _tasksViewModel.Tasks.First().Name);

            // Add second task
            var taskViewModel2 = new TaskViewModel { Name = "task2" };

            _tasksViewModel.NewTaskViewModel = taskViewModel2;
            _tasksViewModel.AddTaskCommand.Execute(null);

            Assert.AreEqual(2, _tasksViewModel.Tasks.Count());
            Assert.AreEqual("task2", _tasksViewModel.Tasks.Skip(1).First().Name);

            // Select first task
            _tasksViewModel.Tasks.First().IsSelected = true;

            // Edit first task
            _tasksViewModel.EditTaskCommand.Execute(null);
            var editedTaskViewModel = _tasksViewModel.NewTaskViewModel;

            Assert.AreEqual(taskViewModel1.Name, editedTaskViewModel.Name);
            Assert.AreEqual(taskViewModel1.TaskFrequency, editedTaskViewModel.TaskFrequency);
            Assert.AreEqual(taskViewModel1.Status, editedTaskViewModel.Status);
            Assert.AreEqual(taskViewModel1.Desk, editedTaskViewModel.Desk);
            Assert.AreEqual(taskViewModel1.Description, editedTaskViewModel.Description);
            Assert.AreEqual(taskViewModel1.IntermediateDate, editedTaskViewModel.IntermediateDate);
            Assert.AreEqual(taskViewModel1.FinalDate, editedTaskViewModel.FinalDate);
            Assert.AreEqual(taskViewModel1.RelatedTo, editedTaskViewModel.RelatedTo);

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
            return new TasksViewModel(tasksContext, new MockMessageBoxService());
        }
    }
}
