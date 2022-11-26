using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Tasks4U.Models;

namespace Tasks4U.ViewModels
{
    public class TaskDateViewModel : ObservableValidator
    {
        // The date that should be written to the model - 
        // the selected date if frequency is Once (or minimum date if date is not selected).
        // Otherwise, the next date that corresponds to the selected recurring date considering the frequeny.
        // For instance, if frequency is EveryWeek and Monday is selected, then TaskDate is next monday.
        // This week we can save dates in the database in the same format
        // and deduce the next recurring date from the starting date and the frequency.
        public DateOnly TaskDate
        {
            get
            {
                if (TaskFrequency == Frequency.Once)
                    return new DateOnly(_dateTime.Year, _dateTime.Month, _dateTime.Day);

                var today = DateOnly.FromDateTime(DateTime.Today);

                switch (TaskFrequency)
                {
                    case Frequency.EveryWeek:
                        return today.AddDays((WeekDay - today.DayOfWeek + 7) % 7);
                    case Frequency.EveryMonth:
                        var matchingDayInThisMonth = new DateOnly(today.Year, today.Month, Day);
                        return matchingDayInThisMonth >= today ? matchingDayInThisMonth : matchingDayInThisMonth.AddMonths(1);
                    case Frequency.EveryYear:
                        var matchingDayInThisYear = new DateOnly(today.Year, (int)Month, DayInMonth);
                        return matchingDayInThisYear >= today ? matchingDayInThisYear : matchingDayInThisYear.AddYears(1);
                }

                return DateOnly.MinValue;
            }
        }

        private Frequency _taskFrequency = Frequency.Once;
        public Frequency TaskFrequency
        {
            get => _taskFrequency;
            set
            {
                _taskFrequency = value;
                OnPropertyChanged(nameof(IsOnceFrequency));
                OnPropertyChanged(nameof(IsEveryWeekFrequency));
                OnPropertyChanged(nameof(IsEveryMonthFrequency));
                OnPropertyChanged(nameof(IsEveryYearFrequency));
            }
        }

        public bool IsOnceFrequency => _taskFrequency == Frequency.Once;
        public bool IsEveryWeekFrequency => _taskFrequency == Frequency.EveryWeek;
        public bool IsEveryMonthFrequency => _taskFrequency == Frequency.EveryMonth;
        public bool IsEveryYearFrequency => _taskFrequency == Frequency.EveryYear;

        // Used when Frequency is Once
        private DateTime _dateTime = DateTime.Today;
        public DateTime Date
        {
            get => _dateTime;
            set => SetProperty(ref _dateTime, value);
        }

        // Used when Frequency is EveryWeek
        private DayOfWeek _weekDay = DayOfWeek.Sunday;
        public DayOfWeek WeekDay
        {
            get => _weekDay;
            set => SetProperty(ref _weekDay, value);
        }

        // Used when Frequency is EveryMonth
        private int _day = 1;
        public int Day
        {
            get => _day;
            set => SetProperty(ref _day, value);
        }

        // Used when Frequency is EveryYear
        private MonthOfYear _month = MonthOfYear.January;
        public MonthOfYear Month
        {
            get => _month;
            set
            {
                SetProperty(ref _month, value);
                OnPropertyChanged(nameof(DaysInMonth));
            }
        }

        public IEnumerable<int> DaysInMonth
        {
            get
            {
                const int year = 1; // 1 is not a leap year, so it will have 28 days for February
                int numOfDays = DateTime.DaysInMonth(year, (int)_month);
                return Enumerable.Range(1, numOfDays);
            }
        }

        private int _dayInMonth = 1;
        public int DayInMonth
        {
            get => _dayInMonth;
            set => SetProperty(ref _dayInMonth, value);
        }

        // Options for comboxes
        public IEnumerable<DayOfWeek> WeekDays { get; } = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();
        public IEnumerable<int> Days1To28 { get; } = Enumerable.Range(1, 28);
        public IEnumerable<MonthOfYear> Months { get; } = Enum.GetValues(typeof(MonthOfYear)).Cast<MonthOfYear>();

        public enum MonthOfYear
        {
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12
        }
    }
}
