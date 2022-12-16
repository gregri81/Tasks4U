using Tasks4U.Models;
using Tasks4U.ViewModels;
using Task = Tasks4U.Models.Task;

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
        public void TasksAddedAndRemoved()
        {
            // Add first task
            var taskViewModel1 = new TaskViewModel { Name = "task1" };

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

            // Remove first task
            _tasksViewModel.Tasks.First().IsSelected = true;

            _tasksViewModel.RemoveSelectedTasksCommand.Execute(null);

            Assert.AreEqual(1, _tasksViewModel.Tasks.Count());
            Assert.AreEqual("task2", _tasksViewModel.Tasks.First().Name);
        }

        [TestMethod]
        public void ShowNewTaskCommandTest()
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
        }

        private static TasksViewModel CreateTasksViewModel()
        {
            var tasksContext = new TasksContext("Data Source=test.db");
            return new TasksViewModel(tasksContext);
        }
    }
}
