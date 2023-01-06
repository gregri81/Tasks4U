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
            var relatedTo = string.Empty;
            var taskFrequency = Frequency.Once;
            var desk = Desk.General;
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
                    case nameof(TaskViewModel.RelatedTo):
                        relatedTo = taskViewModel.RelatedTo;
                        break;
                    case nameof(TaskViewModel.Desk):
                        desk = taskViewModel.Desk;
                        break;
                }
            };

            taskViewModel.Name = "name";
            taskViewModel.Description = "description";
            taskViewModel.RelatedTo = "donor";
            taskViewModel.Desk = Desk.USA;
            taskViewModel.TaskFrequency = Frequency.EveryWeek;

            Assert.AreEqual(name, taskViewModel.Name);
            Assert.AreEqual(description, taskViewModel.Description);
            Assert.AreEqual(relatedTo, taskViewModel.RelatedTo);
            Assert.AreEqual(desk, taskViewModel.Desk);
            Assert.AreEqual(taskFrequency, taskViewModel.TaskFrequency);

            // Test that Clear function works
            taskViewModel.Clear();
            var today = DateOnly.FromDateTime(DateTime.Today);
            var tomorrow = today.AddDays(1);
            Assert.AreEqual(today.ToString(), taskViewModel.IntermediateDateViewModel.DateText);
            Assert.AreEqual(tomorrow.ToString(), taskViewModel.FinalDateViewModel.DateText);
            Assert.AreEqual(string.Empty, taskViewModel.Name);
            Assert.AreEqual(string.Empty, taskViewModel.Description);
            Assert.AreEqual(Frequency.Once, taskViewModel.TaskFrequency);
        }

        [TestMethod]
        public void ValidateTest()
        {
            var taskViewModel = new TaskViewModel() { Name = "name", };

            var today = DateOnly.FromDateTime(DateTime.Today);
            var tomorrow = today.AddDays(1);

            taskViewModel.IntermediateDateViewModel.DateText = today.ToString();
            taskViewModel.FinalDateViewModel.DateText = tomorrow.ToString();

            Assert.IsTrue(taskViewModel.IsValid());

            // Assert that taskViewModel is not valid when the name is empty
            taskViewModel.Name = string.Empty;
            Assert.IsFalse(taskViewModel.IsValid());
            taskViewModel.Name = "name";
            Assert.IsTrue(taskViewModel.IsValid());

            // Assert that taskViewModel is not valid when intermediate date is not valid
            taskViewModel.IntermediateDateViewModel.DateText = today.ToString() + "a";
            Assert.IsFalse(taskViewModel.IsValid());
            taskViewModel.IntermediateDateViewModel.DateText = today.ToString();
            Assert.IsTrue(taskViewModel.IsValid());

            // Assert that taskViewModel is not valid when final date is not valid
            taskViewModel.FinalDateViewModel.DateText = tomorrow.ToString() + "a";
            Assert.IsFalse(taskViewModel.IsValid());
            taskViewModel.FinalDateViewModel.DateText = tomorrow.ToString();
            Assert.IsTrue(taskViewModel.IsValid());

            // Assert that when we disable validation, it's disabled for both intermediate-date and final-date
            taskViewModel.DisableDateValidation(true);
            Assert.IsTrue(taskViewModel.IntermediateDateViewModel.IsDateValidationDisabled);
            Assert.IsTrue(taskViewModel.FinalDateViewModel.IsDateValidationDisabled);

            // Assert that when we enable validation, it's disabled for both intermediate-date and final-date
            taskViewModel.DisableDateValidation(false);
            Assert.IsFalse(taskViewModel.IntermediateDateViewModel.IsDateValidationDisabled);
            Assert.IsFalse(taskViewModel.FinalDateViewModel.IsDateValidationDisabled);
        }

        private void TaskViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
