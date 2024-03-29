﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using Tasks4U.Models;


namespace Tasks4U.ViewModels
{
    public class TaskViewModel : ObservableValidator
    {
        public event Action? IsValidChanged;

        public TaskViewModel()
        {
            // Set name explicitly in order to cause validation
            _name = Name = "";

            var intermediateDateValidator = (DateOnly date) =>
                _isIntermediateDateEnabled && !FinalDateViewModel.HasErrors && date >= FinalDate
                ? "Must be earlier than final date"
                : string.Empty;

            IntermediateDateViewModel = new TaskDateViewModel(intermediateDateValidator);

            FinalDateViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(FinalDateViewModel.DateText))
                    IntermediateDateViewModel.ValidateDate();
            };

            FinalDateViewModel.ErrorsChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(FinalDateViewModel.DateText))
                    IntermediateDateViewModel.ValidateDate();
            };

            ErrorsChanged += (s, e) => IsValidChanged?.Invoke();
            IntermediateDateViewModel.ErrorsChanged += (s, e) => IsValidChanged?.Invoke();
            FinalDateViewModel.ErrorsChanged += (s, e) => IsValidChanged?.Invoke();
        }

        #region properties

        public static Array FrequencyValues => Enum.GetValues(typeof(Frequency));

        public static Array StatusValues => Enum.GetValues(typeof(TaskStatus));

        public static Array DeskValues => Enum.GetValues(typeof(Desk));
        
        public static Array TypeValues => Enum.GetValues(typeof(TaskType));

        public TaskDateViewModel IntermediateDateViewModel { get; }

        public TaskDateViewModel FinalDateViewModel { get; } = new TaskDateViewModel();

        private string _name;
        [Required(ErrorMessage = "Subject is required")]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, true);
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _relatedTo = string.Empty;
        public string RelatedTo
        {
            get => _relatedTo;
            set => SetProperty(ref _relatedTo, value);
        }

        private Frequency _taskFrequency = Frequency.Once;
        public Frequency TaskFrequency
        {
            get => _taskFrequency;
            set
            {
                SetProperty(ref _taskFrequency, value);
                IntermediateDateViewModel.TaskFrequency = value;
                FinalDateViewModel.TaskFrequency = value;
                IntermediateDateViewModel.ValidateDate();
            }
        }

        private TaskStatus _status;
        public TaskStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private Desk _desk = Desk.General;
        public Desk Desk
        {
            get => _desk;
            set => SetProperty(ref _desk, value);
        }

        private TaskType _taskType = TaskType.None;
        public TaskType TaskType
        {
            get => _taskType;
            set => SetProperty(ref _taskType, value);
        }

        private bool _isIntermediateDateEnabled;
        public bool IsIntermediateDateEnabled
        {
            get => _isIntermediateDateEnabled;
            set
            {
                SetProperty(ref _isIntermediateDateEnabled, value);
                IntermediateDateViewModel.ValidateDate();
            }
        }

        public DateOnly IntermediateDate
        {
            get => _isIntermediateDateEnabled ? IntermediateDateViewModel.TaskDate : DateOnly.MinValue;

            set
            {
                if (value == DateOnly.MinValue)
                {
                    IsIntermediateDateEnabled = false;
                }
                else
                {
                    IsIntermediateDateEnabled = true;
                    IntermediateDateViewModel.TaskDate = value;
                }
            }
        }

        public DateOnly FinalDate
        {
            get => FinalDateViewModel.TaskDate;
            set => FinalDateViewModel.TaskDate = value;
        }

        private bool _keyboardFocusOnTextBox;
        public bool KeyboardFocusOnTextBox
        {
            get => _keyboardFocusOnTextBox;
            set => SetProperty(ref _keyboardFocusOnTextBox, value);       
        }

        private FlowDirection _nameDirection;
        public FlowDirection NameDirection
        {
            get => _nameDirection;
            set => SetProperty(ref _nameDirection, value);
        }

        private FlowDirection _relatedToDirection;
        public FlowDirection RelatedToDirection
        {
            get => _relatedToDirection;
            set => SetProperty(ref _relatedToDirection, value);
        }

        private FlowDirection _descriptionDirection;
        public FlowDirection DescriptionDirection
        {
            get => _descriptionDirection;
            set => SetProperty(ref _descriptionDirection, value);
        }
        #endregion

        #region methods

        public void Clear()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            IntermediateDateViewModel.DateText = today.ToString();
            FinalDateViewModel.DateText = today.AddDays(1).ToString();
            Name = string.Empty;
            Description = string.Empty;
            TaskFrequency = Frequency.Once;
            RelatedTo = string.Empty;
            Desk = Desk.General;
            TaskType = TaskType.None;
            Status = TaskStatus.InProgress;
            NameDirection = FlowDirection.RightToLeft;
            RelatedToDirection = FlowDirection.RightToLeft;
            DescriptionDirection = FlowDirection.RightToLeft;
        }

        public bool IsValid() => !HasErrors && !IntermediateDateViewModel.HasErrors && !FinalDateViewModel.HasErrors;

        public void DisableDateValidation(bool value)
        {
            IntermediateDateViewModel.IsDateValidationDisabled = value;
            FinalDateViewModel.IsDateValidationDisabled = value;
        }

        public Memento CreateMemento()
        {
            return new Memento(
                            Name,
                            Description,
                            RelatedTo,
                            TaskFrequency,
                            Status,
                            Desk,
                            TaskType,
                            IsIntermediateDateEnabled,
                            IntermediateDate,
                            FinalDate);
        }

        #endregion

        public record Memento(string Name, 
                              string Description, 
                              string RelatedTo, 
                              Frequency TaskFrequency, 
                              TaskStatus Status, 
                              Desk Desk,
                              TaskType TaskType,
                              bool IsIntermediateDateEnabled, 
                              DateOnly IntermediateDate, 
                              DateOnly FinalDate);
    }
}
