using Task = Tasks4U.Models.Task;
using Tasks4U.Models;
using TaskStatus = Tasks4U.Models.TaskStatus;

namespace Task4UTests
{
    [TestClass]
    public class TaskTests
    {
        [TestMethod]
        public void CorrespondsToDateTests()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // One-time task
            var task = new Task("")
            {
                IntermediateDate = DateOnly.FromDateTime(today),
                FinalDate = DateOnly.FromDateTime(tomorrow),
                TaskFrequency = Frequency.Once
            };

            Assert.IsTrue(task.ShouldShowIntermediateNotification(today));
            Assert.IsTrue(task.ShouldShowFinalNotification(tomorrow));
            Assert.IsFalse(task.ShouldShowFinalNotification(today));
            Assert.IsFalse(task.ShouldShowIntermediateNotification(tomorrow));

            // Weekly task
            task.TaskFrequency = Frequency.EveryWeek;

            Assert.IsTrue(task.ShouldShowIntermediateNotification(today.AddDays(7)));
            Assert.IsTrue(task.ShouldShowFinalNotification(tomorrow.AddDays(14)));
            Assert.IsTrue(task.ShouldShowIntermediateNotification(today.AddDays(21)));
            Assert.IsTrue(task.ShouldShowFinalNotification(tomorrow.AddDays(70)));

            task.Status = TaskStatus.Finished;
            Assert.IsFalse(task.ShouldShowFinalNotification(today.AddDays(21)));

            task.Status = TaskStatus.InProgress;
            Assert.IsTrue(task.ShouldShowFinalNotification(today.AddDays(21)));


            Assert.IsFalse(task.ShouldShowIntermediateNotification(tomorrow.AddDays(70)));

            // No intermediate date
            task.IntermediateDate = DateOnly.MinValue;
            Assert.IsFalse(task.ShouldShowIntermediateNotification(task.IntermediateDate.ToDateTime(TimeOnly.MinValue)));

            // Monthly task
            var secondDayOfMonth = new DateTime(today.Year, today.Month, 2);
            var forthDayOfMonth = new DateTime(today.Year, today.Month, 4);

            task.TaskFrequency = Frequency.EveryMonth;
            task.IntermediateDate = DateOnly.FromDateTime(secondDayOfMonth);
            task.FinalDate = DateOnly.FromDateTime(forthDayOfMonth);

            Assert.IsTrue(task.ShouldShowIntermediateNotification(secondDayOfMonth.AddMonths(1)));
            Assert.IsTrue(task.ShouldShowFinalNotification(forthDayOfMonth.AddMonths(1)));
            Assert.IsTrue(task.ShouldShowIntermediateNotification(secondDayOfMonth.AddYears(1)));
            Assert.IsTrue(task.ShouldShowFinalNotification(forthDayOfMonth.AddYears(1)));
            Assert.IsFalse(task.ShouldShowIntermediateNotification(forthDayOfMonth.AddYears(1)));

            task.Status = Tasks4U.Models.TaskStatus.Finished;
            Assert.IsFalse(task.ShouldShowFinalNotification(secondDayOfMonth.AddYears(1)));

            task.Status = Tasks4U.Models.TaskStatus.NotStarted;
            Assert.IsTrue(task.ShouldShowFinalNotification(secondDayOfMonth.AddYears(1)));

            // Yearly task
            task.TaskFrequency = Frequency.EveryYear;
            task.IntermediateDate = DateOnly.FromDateTime(today);
            task.FinalDate = DateOnly.FromDateTime(tomorrow);

            Assert.IsTrue(task.ShouldShowIntermediateNotification(today.AddYears(1)));
            Assert.IsTrue(task.ShouldShowFinalNotification(tomorrow.AddYears(10)));
            Assert.IsTrue(task.ShouldShowIntermediateNotification(today.AddYears(21)));
            Assert.IsTrue(task.ShouldShowFinalNotification(tomorrow.AddYears(70)));
            Assert.IsFalse(task.ShouldShowIntermediateNotification(tomorrow.AddYears(21)));

            task.Status = Tasks4U.Models.TaskStatus.Finished;
            Assert.IsFalse(task.ShouldShowFinalNotification(today.AddYears(70)));

            task.Status = Tasks4U.Models.TaskStatus.Pending;
            Assert.IsTrue(task.ShouldShowFinalNotification(today.AddYears(70)));
        }

        [DynamicData(nameof(RenewRecurringTaskData), DynamicDataSourceType.Method)]
        [TestMethod]
        public void RenewRecurringTaskTests(Frequency taskFrequency, DateTime prevTaskDate)
        {
            var beforeTaskCreation = DateTime.Now;

            var task = new Task("")
            {
                TaskFrequency = taskFrequency,
                FinalDate = DateOnly.FromDateTime(prevTaskDate),
                Status = TaskStatus.Pending
            };

            // The renewal time of a newly created task is its creation time
            Assert.IsTrue(task.LastRenewalTime >= beforeTaskCreation &&
                          task.LastRenewalTime <= DateTime.Now);

            Assert.IsTrue(task.Status == TaskStatus.Pending);

            // try renewing the new task - it shouldn't be renewed because it was just created
            var renewalTime = task.LastRenewalTime;
            task.RenewRecurringTaskIfNeeded();
            Assert.IsTrue(task.LastRenewalTime == renewalTime);
            Assert.IsTrue(task.Status == TaskStatus.Pending);

            // Now the renewal time is exactly on the date of the task -
            // we shouldn't renew it because it has already been renewed this year
            task.LastRenewalTime = prevTaskDate;
            task.RenewRecurringTaskIfNeeded();
            Assert.IsFalse(task.LastRenewalTime == DateTime.MinValue);
            Assert.IsTrue(task.Status == TaskStatus.Pending);

            // Now the renewal time is a millisecond before the date of the task -
            // we should renew it because it hasn't been renewed this year
            var justBeforeTaskDate = prevTaskDate.AddMilliseconds(-1);
            task.LastRenewalTime = justBeforeTaskDate;
            task.RenewRecurringTaskIfNeeded();
            Assert.IsFalse(task.LastRenewalTime == justBeforeTaskDate);
            Assert.IsTrue(task.Status == TaskStatus.NotStarted);
        }

        private static IEnumerable<object[]> RenewRecurringTaskData()
        {
            var today = DateTime.Today;

            // Weekly task
            for (int i = 0; i < 7; i++)
                yield return new object[] { Frequency.EveryWeek, today.AddDays(-i) };

            // Monthly task
            for (int day = 1; day <= 28; day++)
            {
                var prevTaskDate = new DateTime(today.Year, today.Month, day);

                if (prevTaskDate > today)
                    prevTaskDate = prevTaskDate.AddMonths(-1);

                yield return new object[] { Frequency.EveryMonth, prevTaskDate };
            }

            // Yearly task
            for (int month = 1; month <= 12; month++)
            {
                for (int day = 1; day <= 28; day++)
                {
                    var prevTaskDate = new DateTime(today.Year, month, day);

                    if (prevTaskDate > today)
                        prevTaskDate = prevTaskDate.AddYears(-1);

                    yield return new object[] { Frequency.EveryYear, prevTaskDate };
                }
            }
        }
    }
}
