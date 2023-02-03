using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;
using Tasks4U.Models;
using Task = Tasks4U.Models.Task;
using TaskStatus = Tasks4U.Models.TaskStatus;

namespace Tasks4U.ViewModels
{
    public class FilterViewModel : ObservableValidator
    {
        public static Array FilterValues => Enum.GetValues(typeof(FilterType));
        public static Array FrequencyValues => Enum.GetValues(typeof(Frequency));

        public static Array StatusValues => Enum.GetValues(typeof(TaskStatus));

        public static Array DeskValues => Enum.GetValues(typeof(Desk));

        public event Action? IsFilterChanged;

        private FilterType _selectedFilter;
        public FilterType SelectedFilter
        {
            get => _selectedFilter;

            set
            {
                SetProperty(ref _selectedFilter, value);

                OnPropertyChanged(nameof(IsTextFilter));
                OnPropertyChanged(nameof(IsDeskFilter));
                OnPropertyChanged(nameof(IsStatusFilter));
                OnPropertyChanged(nameof(IsDateFilter));

                IsFilterChanged?.Invoke();
            }
        }
        public bool IsTextFilter => _selectedFilter == FilterType.Text;

        public bool IsDeskFilter => _selectedFilter == FilterType.Desk;

        public bool IsStatusFilter => _selectedFilter == FilterType.Status;

        public bool IsDateFilter => _selectedFilter == FilterType.Date;

        private string _text = string.Empty;
        public string Text
        {
            get => _text;

            set
            {
                SetProperty(ref _text, value);
                IsFilterChanged?.Invoke();
            }
        }

        private TaskStatus _status;
        public TaskStatus Status
        {
            get => _status;

            set
            {
                SetProperty(ref _status, value);
                IsFilterChanged?.Invoke();
            }
        }

        private Desk _desk = Desk.General;
        public Desk Desk
        {
            get => _desk;

            set
            {
                SetProperty(ref _desk, value);
                IsFilterChanged?.Invoke();
            }
        }

        private string _startDateText = DateTime.Now.ToString();
        private DateOnly _startDate = DateOnly.FromDateTime(DateTime.Today);
        [CustomValidation(typeof(FilterViewModel), nameof(ValidateDateText))]
        public string StartDateText
        {
            get => _startDateText;

            set
            {
                SetProperty(ref _startDateText, value, true);
                DateOnly.TryParseExact(_startDateText, TaskDateViewModel.DateFormat, out _startDate);
                OnPropertyChanged(nameof(StartDate));

                if (_startDate > _endDate)
                    EndDateText = _startDateText; // This will also invoke IsFilterChanged
                else
                    IsFilterChanged?.Invoke();
            }
        }

        private string _endDateText = DateTime.Now.ToString();
        private DateOnly _endDate = DateOnly.FromDateTime(DateTime.Today);
        [CustomValidation(typeof(FilterViewModel), nameof(ValidateDateText))]
        public string EndDateText
        {
            get => _endDateText;

            set
            {
                SetProperty(ref _endDateText, value, true);
                DateOnly.TryParseExact(_endDateText, TaskDateViewModel.DateFormat, out _endDate);
                IsFilterChanged?.Invoke();
            }
        }

        public DateTime StartDate => _startDate.ToDateTime(TimeOnly.MinValue);

        public bool IsTaskFilteredIn(Task task)
        {
            return SelectedFilter switch
            {
                FilterType.None => true,

                FilterType.Text =>
                    task.Name.Contains(_text, StringComparison.OrdinalIgnoreCase) ||
                    task.RelatedTo.Contains(_text, StringComparison.OrdinalIgnoreCase) ||
                    task.Description.Contains(_text, StringComparison.OrdinalIgnoreCase),

                FilterType.Desk => task.Desk == _desk,

                FilterType.Status => task.Status == _status,

                FilterType.Date =>
                    task.TaskFrequency == Frequency.Once &&
                    (task.FinalDate >= _startDate && task.FinalDate <= _endDate ||
                     task.IntermediateDate >= _startDate && task.IntermediateDate <= _endDate),

                _ => false,
            };
        }

        public static ValidationResult? ValidateDateText(string dateText, ValidationContext context)
        {
            if (!DateOnly.TryParseExact(dateText, TaskDateViewModel.DateFormat, out _))
                return new ValidationResult("Date is not in valid format");

            return null;
        }

        public enum FilterType { None, Text, Date, Desk, Status }
    }
}
