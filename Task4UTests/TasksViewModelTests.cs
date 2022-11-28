using System.Threading.Tasks;
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
            var taskViewModels = new TaskViewModel[2];
            
            // First task added
            taskViewModels[0] = new TaskViewModel { Name = "Task1", Description = "Description1", TaskFrequency = Frequency.EveryWeek };

            _tasksViewModel.NewTaskViewModel = taskViewModels[0];
            _tasksViewModel.AddTaskCommand.Execute(null);

            Assert.AreEqual(_tasksViewModel.Tasks.Count(), 1);

            // Second added
            taskViewModels[1] = new TaskViewModel { Name = "Task2", Description = "description2", TaskFrequency = Frequency.Once };
            _tasksViewModel.NewTaskViewModel = taskViewModels[1];
            _tasksViewModel.AddTaskCommand.Execute(null);

            Assert.AreEqual(_tasksViewModel.Tasks.Count(), 2);

            // Assert basic values of both tasks
            int i = 0;
            foreach (var task in _tasksViewModel.Tasks)
            {
                Assert.AreEqual(task.Name, taskViewModels[i].Name);
                Assert.AreEqual(task.Description, taskViewModels[i].Description);
                Assert.AreEqual(task.TaskFrequency, taskViewModels[i].TaskFrequency);
                i++;
            }

            // Select second task
            _tasksViewModel.Tasks.First().IsSelected = true;

            // Remove selected tasks
            _tasksViewModel.RemoveSelectedTasksCommand.Execute(null);

            // Assert that only the selected task is removed
            Assert.AreEqual(_tasksViewModel.Tasks.Count(), 1);
            var remainingTask = _tasksViewModel.Tasks.First();
            Assert.AreEqual(remainingTask.Name, _tasksViewModel.Tasks.First().Name);
            Assert.AreEqual(remainingTask.Description, _tasksViewModel.Tasks.First().Description);
            Assert.AreEqual(remainingTask.TaskFrequency, _tasksViewModel.Tasks.First().TaskFrequency);
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

        private static TasksViewModel CreateTasksViewModel() =>
            new TasksViewModel(new TasksContext("Data Source=test.db"));
    }
}
