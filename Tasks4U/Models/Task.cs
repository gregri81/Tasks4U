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

        [NotMapped]
        public DateOnly LastIntermediateNotificationDate { get; set; }

        [NotMapped]
        public DateOnly LastFinalNotificationDate { get; set; }

        // Yes, it's not MVVM to store IsSelected and IsFilteredOut properties in the model.
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

        public bool ShouldShowIntermediateNotification(DateOnly currentDate) =>
            IntermediateDate > DateOnly.MinValue &&
            CorrespondsToDate(IntermediateDate, currentDate, LastIntermediateNotificationDate);

        public bool ShouldShowFinalNotification(DateOnly currentDate) =>
            CorrespondsToDate(FinalDate, currentDate, LastFinalNotificationDate);

        private bool CorrespondsToDate(DateOnly taskDate,
                                        DateOnly currentDate,
                                        DateOnly lastNotificationDate)
        {
            if (currentDate <= lastNotificationDate)
                return false;

            switch (TaskFrequency)
            {
                case Frequency.Once:
                    return taskDate == currentDate;
                case Frequency.EveryWeek:
                    return taskDate.DayOfWeek == currentDate.DayOfWeek;
                case Frequency.EveryMonth:
                    return taskDate.Day == currentDate.Day;
                case Frequency.EveryYear:
                    return taskDate.Month == currentDate.Month && taskDate.Day == currentDate.Day;
                default:
                    return false;
            }
        }
    }
}
