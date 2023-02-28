using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tasks4U.Models
{
    public enum Desk { General, USA, UK, Canada };
    public enum TaskStatus { NotStarted, InProgress, Pending, Finished };

    public enum Frequency { Once, EveryWeek, EveryMonth, EveryYear };

    public class Task : ObservableObject
    {
        public Task(string name) => Name = name;

        public int ID { get; set; }

        public string Name { get; set; }
        public string RelatedTo { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Desk Desk { get; set; }
        public Frequency TaskFrequency { get; set; }
        public DateOnly IntermediateDate { get; set; }
        public DateOnly FinalDate { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime LastRenewalTime { get; set; } = DateTime.Now;

        [NotMapped]
        public DateTime LastNotificationTime { get; set; }

        // Yes, it's not MVVM to store IsSelected property in the model.
        // But sometimes rules just have to be broken in order to simplify the code...
        private bool _isSelected;
        [NotMapped]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                IsUnmappedRowPropertyChanged?.Invoke();
            }
        }

        public event Action? IsUnmappedRowPropertyChanged;

        // Clear status and raises a callback.
        // This is needed when status is clear due to renewal of a recurring task - 
        // in this case we have to clear in the GUI as well
        public void ClearStatus()
        {
            Status = TaskStatus.NotStarted;
            OnPropertyChanged(nameof(Status));
        }

        public bool RenewRecurringTaskIfNeeded()
        {
            if (LastRenewalTime < LastDateOfReccuringTask.ToDateTime(TimeOnly.MinValue))
            {
                LastRenewalTime = DateTime.Now;
                Status = TaskStatus.NotStarted;
                return true;
            }

            return false;
        }

        // For recurring tasks its different than FinalDate (because FinalDate doesn't change)
        public DateOnly NextDateOfTask
        {
            get
            {
                if (TaskFrequency == Frequency.Once)
                    return FinalDate;

                var today = DateOnly.FromDateTime(DateTime.Today);
                var date = today;

                switch (TaskFrequency)
                {
                    case Frequency.EveryWeek:
                        while (date.DayOfWeek != FinalDate.DayOfWeek)
                            date = date.AddDays(1);

                    break;

                    case Frequency.EveryMonth:
                        date = new DateOnly(date.Year, date.Month, FinalDate.Day);

                        if (date < today)
                            date = date.AddMonths(1);

                        break;

                    case Frequency.EveryYear:
                        date = new DateOnly(today.Year, FinalDate.Month, FinalDate.Day);

                        if (date < today)
                            date = date.AddYears(1);

                        break;
                }

                return date;
            }
        }

        public bool IsDayOfFinalDate(DateOnly date)
        {
            return TaskFrequency switch
            {
                Frequency.Once => date == FinalDate,
                Frequency.EveryWeek => date.DayOfWeek == FinalDate.DayOfWeek,
                Frequency.EveryMonth => date.Day == FinalDate.Day,
                Frequency.EveryYear => date.Month == FinalDate.Month && date.Day == FinalDate.Day,
                _ => false
            };
        }

        private DateOnly LastDateOfReccuringTask
        {
            get
            {
                var nextDateOfTask = NextDateOfTask;

                if (nextDateOfTask == DateOnly.FromDateTime(DateTime.Today))
                    return nextDateOfTask;

                switch (TaskFrequency)
                {
                    case Frequency.EveryWeek:
                        return nextDateOfTask.AddDays(-7);

                    case Frequency.EveryMonth:
                        return nextDateOfTask.AddMonths(-1);

                    case Frequency.EveryYear:
                        return nextDateOfTask.AddYears(-1);

                    default:
                        return DateOnly.MinValue;
                }
            }
        }      

        public bool ShouldShowIntermediateNotification(DateTime currentTime)
        {
            if (IntermediateDate == DateOnly.MinValue)
                return false;

            if (Status == TaskStatus.Finished)
                return false;

            if (currentTime - LastNotificationTime <= TimeSpan.FromHours(1))
                return false;

            return TaskFrequency switch
            {
                Frequency.Once => IntermediateDate == DateOnly.FromDateTime(currentTime),
                Frequency.EveryWeek => IntermediateDate.DayOfWeek == currentTime.DayOfWeek,
                Frequency.EveryMonth => IntermediateDate.Day == currentTime.Day,
                Frequency.EveryYear => IntermediateDate.Month == currentTime.Month &&
                                        IntermediateDate.Day == currentTime.Day,
                _ => false
            };
        }

        public bool ShouldShowFinalNotification(DateTime currentTime)
        {
            if (Status == TaskStatus.Finished)
                return false;

            if (currentTime - LastNotificationTime <= TimeSpan.FromHours(1))
                return false;

            return DateOnly.FromDateTime(currentTime) >= FinalDate;
        }
    }
}
