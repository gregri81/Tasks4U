using Tasks4U.ViewModels;
using Tasks4U.Models;
using Task = Tasks4U.Models.Task;

namespace Task4UTests
{
    [TestClass]
    public class TasksContextTest
    {
        private TasksContext _tasksContext = new TasksContext("Data Source=test.db");

        [TestInitialize]
        public void DeleteTasks()
        {
            _tasksContext.Database.EnsureCreated();
            _tasksContext.Tasks.RemoveRange(_tasksContext.Tasks);
            _tasksContext.SaveChanges();
        }

        [TestMethod]
        public void NoTasksIfModelIsEmpty()
        {
            Assert.AreEqual(_tasksContext.Tasks.Count(), 0);
            Assert.AreEqual(_tasksContext.Tasks.Local.Count(), 0);
        }

        [TestMethod]
        public void TasksAddedAndRemoved()
        {
            var task1 = new Task("test1");
            var task2 = new Task("test2");

            var tasksCount = _tasksContext.Tasks.Count();
            Assert.AreEqual(tasksCount, 0);

            _tasksContext.Tasks.Local.CollectionChanged += (s, e) =>
               tasksCount = _tasksContext.Tasks.Local.Count();

            _tasksContext.Tasks.Add(task1);
            Assert.AreEqual(tasksCount, 1);

            _tasksContext.Tasks.Add(task2);
            Assert.AreEqual(tasksCount, 2);

            _tasksContext.Tasks.Remove(task1);
            Assert.AreEqual(tasksCount, 1);

            _tasksContext.Tasks.Remove(task2);
            Assert.AreEqual(tasksCount, 0);
        }
    }
}