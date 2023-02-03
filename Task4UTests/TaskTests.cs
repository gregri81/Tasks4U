using Task = Tasks4U.Models.Task;
using Tasks4U.Models;

namespace Task4UTests
{
    [TestClass]
    public class TaskTests
    {
        [TestMethod]
        public void CorrespondsToDateTests()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var tomorrow = today.AddDays(1);

            // One-time task
            var task = new Task("") 
            { 
                IntermediateDate = today, 
                FinalDate = tomorrow, 
                TaskFrequency = Frequency.Once 
            };

            Assert.IsTrue(task.ShouldShowIntermediateNotification(today));
            Assert.IsTrue(task.ShouldShowFinalNotification(tomorrow));
            Assert.IsFalse(task.ShouldShowFinalNotification(today));
            Assert.IsFalse(task.ShouldShowIntermediateNotification(tomorrow));

            // Task is not shown again if it was shown today
            task.LastIntermediateNotificationDate = today;
            Assert.IsFalse(task.ShouldShowIntermediateNotification(today));
            Assert.IsTrue(task.ShouldShowFinalNotification(tomorrow));
            Assert.IsFalse(task.ShouldShowFinalNotification(today));
            Assert.IsFalse(task.ShouldShowIntermediateNotification(tomorrow));
            task.LastIntermediateNotificationDate = DateOnly.MinValue;

            task.LastFinalNotificationDate = tomorrow;
            Assert.IsTrue(task.ShouldShowIntermediateNotification(today));
            Assert.IsFalse(task.ShouldShowFinalNotification(tomorrow));
            Assert.IsFalse(task.ShouldShowFinalNotification(today));
            Assert.IsFalse(task.ShouldShowIntermediateNotification(tomorrow));
            task.LastFinalNotificationDate = DateOnly.MinValue;

            // Weekly task
            task.TaskFrequency = Frequency.EveryWeek;

            Assert.IsTrue(task.ShouldShowIntermediateNotification(today.AddDays(7)));
            Assert.IsTrue(task.ShouldShowFinalNotification(tomorrow.AddDays(14)));
            Assert.IsTrue(task.ShouldShowIntermediateNotification(today.AddDays(21)));
            Assert.IsTrue(task.ShouldShowFinalNotification(tomorrow.AddDays(70)));
            Assert.IsFalse(task.ShouldShowFinalNotification(today.AddDays(21)));
            Assert.IsFalse(task.ShouldShowIntermediateNotification(tomorrow.AddDays(70)));

            // No intermediate date
            task.IntermediateDate = DateOnly.MinValue;
            Assert.IsFalse(task.ShouldShowIntermediateNotification(task.IntermediateDate));

            // Monthly task
            var secondDayOfMonth = new DateOnly(today.Year, today.Month, 2);
            var forthDayOfMonth = new DateOnly(today.Year, today.Month, 4);

            task.TaskFrequency = Frequency.EveryMonth;
            task.IntermediateDate = secondDayOfMonth;
            task.FinalDate = forthDayOfMonth;

            Assert.IsTrue(task.ShouldShowIntermediateNotification(secondDayOfMonth.AddMonths(1)));
            Assert.IsTrue(task.ShouldShowFinalNotification(forthDayOfMonth.AddMonths(1)));
            Assert.IsTrue(task.ShouldShowIntermediateNotification(secondDayOfMonth.AddYears(1)));
            Assert.IsTrue(task.ShouldShowFinalNotification(forthDayOfMonth.AddYears(1)));
            Assert.IsFalse(task.ShouldShowIntermediateNotification(forthDayOfMonth.AddYears(1)));
            Assert.IsFalse(task.ShouldShowFinalNotification(secondDayOfMonth.AddYears(1)));

            // Yearly task
            task.TaskFrequency = Frequency.EveryYear;
            task.IntermediateDate = today;
            task.FinalDate = tomorrow;

            Assert.IsTrue(task.ShouldShowIntermediateNotification(today.AddYears(1)));
            Assert.IsTrue(task.ShouldShowFinalNotification(tomorrow.AddYears(10)));
            Assert.IsTrue(task.ShouldShowIntermediateNotification(today.AddYears(21)));
            Assert.IsTrue(task.ShouldShowFinalNotification(tomorrow.AddYears(70)));
            Assert.IsFalse(task.ShouldShowIntermediateNotification(tomorrow.AddYears(21)));
            Assert.IsFalse(task.ShouldShowFinalNotification(today.AddYears(70)));
        }
    }
}
