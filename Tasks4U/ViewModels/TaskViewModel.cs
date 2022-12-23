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
    using FrequencyValue = TaskViewModel.NameAndValue<Frequency>;
    using DeskValue = TaskViewModel.NameAndValue<Desk>;

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

        private Desk _desk = Desk.General;
        public Desk Desk
        {
            get => _desk;
            set=> SetProperty(ref _desk, value);
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

        public DateOnly IntermediateDate => 
            _isIntermediateDateEnabled ? IntermediateDateViewModel.TaskDate : DateOnly.MinValue;

        public DateOnly FinalDate => FinalDateViewModel.TaskDate;

        public IEnumerable<FrequencyValue> FrequencyValues { get; } = new FrequencyValue[]
        {
            new FrequencyValue("Once", Frequency.Once),
            new FrequencyValue("Every Week", Frequency.EveryWeek),
            new FrequencyValue("Every Month", Frequency.EveryMonth),
            new FrequencyValue("Every Year", Frequency.EveryYear)
        };

        public IEnumerable<DeskValue> DeskValues { get; } = new DeskValue[]
        {
            new DeskValue("General", Desk.General),
            new DeskValue("USA", Desk.USA),
            new DeskValue("UK", Desk.UK),
            new DeskValue("Canada", Desk.Canada)
        };

        public class NameAndValue<T>
        {
            public NameAndValue(string name, T value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; set; }
            public T Value { get; set; }
        }
    }
}
