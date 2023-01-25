using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks4U.Models
{
    public enum Desk { General, USA, UK, Canada };
    public enum TaskStatus { NotStarted, InProgress, Pending, Finished };

    public enum Frequency { Once, EveryWeek, EveryMonth, EveryYear };

    public class Task: ObservableObject
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

        private bool _isFilteredOut;
        [NotMapped]
        public bool IsFilteredOut
        {
            get => _isFilteredOut;
            set
            {
                SetProperty(ref _isFilteredOut, value);
                IsUnmappedRowPropertyChanged?.Invoke();                
            }
        }

        public event Action? IsUnmappedRowPropertyChanged;

        public bool CorrespondsToIntermediateDate(DateOnly currentDate) => 
            CorrespondsToDate(IntermediateDate, currentDate);

        public bool CorrespondsToFinalDate(DateOnly currentDate) =>
            CorrespondsToDate(FinalDate, currentDate);

        private bool CorrespondsToDate(DateOnly taskDate, DateOnly currentDate)
        {
            switch(TaskFrequency)
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
