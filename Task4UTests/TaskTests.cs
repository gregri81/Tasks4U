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

            Assert.IsTrue(task.CorrespondsToIntermediateDate(today));
            Assert.IsTrue(task.CorrespondsToFinalDate(tomorrow));
            Assert.IsFalse(task.CorrespondsToFinalDate(today));
            Assert.IsFalse(task.CorrespondsToIntermediateDate(tomorrow));

            // Weekly task
            task.TaskFrequency = Frequency.EveryWeek;

            Assert.IsTrue(task.CorrespondsToIntermediateDate(today.AddDays(7)));
            Assert.IsTrue(task.CorrespondsToFinalDate(tomorrow.AddDays(14)));
            Assert.IsTrue(task.CorrespondsToIntermediateDate(today.AddDays(21)));
            Assert.IsTrue(task.CorrespondsToFinalDate(tomorrow.AddDays(70)));
            Assert.IsFalse(task.CorrespondsToFinalDate(today.AddDays(21)));
            Assert.IsFalse(task.CorrespondsToIntermediateDate(tomorrow.AddDays(70)));

            // Monthly task
            var secondDayOfMonth = new DateOnly(today.Year, today.Month, 2);
            var forthDayOfMonth = new DateOnly(today.Year, today.Month, 4);

            task.TaskFrequency = Frequency.EveryMonth;
            task.IntermediateDate = secondDayOfMonth;
            task.FinalDate = forthDayOfMonth;

            Assert.IsTrue(task.CorrespondsToIntermediateDate(secondDayOfMonth.AddMonths(1)));
            Assert.IsTrue(task.CorrespondsToFinalDate(forthDayOfMonth.AddMonths(1)));
            Assert.IsTrue(task.CorrespondsToIntermediateDate(secondDayOfMonth.AddYears(1)));
            Assert.IsTrue(task.CorrespondsToFinalDate(forthDayOfMonth.AddYears(1)));
            Assert.IsFalse(task.CorrespondsToIntermediateDate(forthDayOfMonth.AddYears(1)));
            Assert.IsFalse(task.CorrespondsToFinalDate(secondDayOfMonth.AddYears(1)));

            // Yearly task
            task.TaskFrequency = Frequency.EveryYear;
            task.IntermediateDate = today;
            task.FinalDate = tomorrow;

            Assert.IsTrue(task.CorrespondsToIntermediateDate(today.AddYears(1)));
            Assert.IsTrue(task.CorrespondsToFinalDate(tomorrow.AddYears(10)));
            Assert.IsTrue(task.CorrespondsToIntermediateDate(today.AddYears(21)));
            Assert.IsTrue(task.CorrespondsToFinalDate(tomorrow.AddYears(70)));
            Assert.IsFalse(task.CorrespondsToIntermediateDate(tomorrow.AddYears(21)));
            Assert.IsFalse(task.CorrespondsToFinalDate(today.AddYears(70)));
        }
    }
}
