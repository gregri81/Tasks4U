using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Tasks4U.Models;


namespace Tasks4U.ViewModels
{
    using TaskStatus = Models.TaskStatus;

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
        }

        public bool IsValid() => !HasErrors && !IntermediateDateViewModel.HasErrors && !FinalDateViewModel.HasErrors;

        public void DisableDateValidation(bool value)
        {
            IntermediateDateViewModel.IsDateValidationDisabled = value;
            FinalDateViewModel.IsDateValidationDisabled = value;
        }

        #endregion
    }
}
