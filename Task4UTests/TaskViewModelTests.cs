using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks4U.Models;
using Tasks4U.ViewModels;

namespace Task4UTests
{
    [TestClass]
    public class TaskViewModelTests
    {
        [TestMethod]
        public void TestProperties()
        {
            var name = string.Empty;
            var description = string.Empty;
            var taskFrequency = Frequency.Once;
            var intermediateDate = DateOnly.MinValue;
            var finalDate = DateOnly.MinValue;

            var taskViewModel = new TaskViewModel();

            taskViewModel.PropertyChanged += (s, e) =>
            {
                switch(e.PropertyName)
                {
                    case nameof(TaskViewModel.Name):
                        name = taskViewModel.Name;
                        break;
                    case nameof(TaskViewModel.Description):
                        description = taskViewModel.Description;
                        break;
                    case nameof(TaskViewModel.TaskFrequency):
                        taskFrequency = taskViewModel.TaskFrequency;
                        break;
                }
            };

            taskViewModel.Name = "name";
            taskViewModel.Description = "description";
            taskViewModel.TaskFrequency = Frequency.EveryWeek;

            Assert.AreEqual(name, taskViewModel.Name);
            Assert.AreEqual(description, taskViewModel.Description);
            Assert.AreEqual(taskFrequency, taskViewModel.TaskFrequency);
        }

        private void TaskViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
