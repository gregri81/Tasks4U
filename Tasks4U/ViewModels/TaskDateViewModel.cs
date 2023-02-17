using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Tasks4U.Models;

namespace Tasks4U.ViewModels
{
    public class TaskDateViewModel : ObservableValidator
    {
        public const string DateFormat = "dd/MM/yyyy";

        private Func<DateOnly, string>? _nonRecurringDateValidation;

        public TaskDateViewModel(Func<DateOnly, string>? nonRecurringDateValidation = null) =>
            _nonRecurringDateValidation = nonRecurringDateValidation;

        #region Properties

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
                {
                    return  DateOnly.TryParseExact(_dateText, DateFormat, out DateOnly date) 
                            ? date 
                            : DateOnly.MinValue;
                }

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

            set
            {
                switch (TaskFrequency)
                {
                    case Frequency.Once:
                        DateText = value.ToString();
                        break;
                    case Frequency.EveryWeek:
                        WeekDay = value.DayOfWeek;
                        break;
                    case Frequency.EveryMonth:
                        Day = value.Day;
                        break;
                    case Frequency.EveryYear:
                        Month = (MonthOfYear)value.Month;
                        DayInMonth = value.Day;
                        break;
                }
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
        private string _dateText = DateTime.Now.ToString();
        [CustomValidation(typeof(TaskDateViewModel), nameof(ValidateDateText))]
        public string DateText
        {
            get => _dateText;
            set => SetProperty(ref _dateText, value, true);
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

        public bool IsDateValidationDisabled { get; set; }

        // Options for comboxes
        public IEnumerable<DayOfWeek> WeekDays { get; } = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();
        public IEnumerable<int> Days1To28 { get; } = Enumerable.Range(1, 28);
        public IEnumerable<MonthOfYear> Months { get; } = Enum.GetValues(typeof(MonthOfYear)).Cast<MonthOfYear>();

        #endregion

        #region methods

        public static ValidationResult? ValidateDateText(string dateText, ValidationContext context)
        {
            var taskDateViewModel = (TaskDateViewModel)context.ObjectInstance;

            if (taskDateViewModel.IsDateValidationDisabled)
                return null;

            if (!DateOnly.TryParseExact(dateText, DateFormat, out DateOnly date))
                return new ValidationResult("Date is not in valid format");            

            if (taskDateViewModel.TaskFrequency == Frequency.Once && 
                taskDateViewModel._nonRecurringDateValidation != null)
            {
                string error = taskDateViewModel._nonRecurringDateValidation.Invoke(date);

                if (error.Length > 0)
                    return new ValidationResult(error);
            }

            return null;
        }

        public void ValidateDate() => ValidateProperty(DateText, nameof(DateText));

        #endregion

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
