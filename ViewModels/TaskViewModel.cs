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
    public class TaskViewModel : ObservableValidator
    {
        public TaskViewModel()
        {
            // Set name explicitly in order to cause validation
            _name = Name = "";
        }

        public TaskDateViewModel IntermediateDateViewModel { get; } = new TaskDateViewModel();
        public TaskDateViewModel FinalDateViewModel { get; } = new TaskDateViewModel();

        private string _name;
        [Required(ErrorMessage = "Subject is Required")]
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

        private Frequency _taskFrequency = Frequency.Once;
        public Frequency TaskFrequency 
        {
            get => _taskFrequency;
            set
            {
                SetProperty(ref _taskFrequency, value);
                IntermediateDateViewModel.TaskFrequency = value;
                FinalDateViewModel.TaskFrequency = value;
            }
        }

        public IEnumerable<FrequencyValue> FrequencyValues { get; } = new FrequencyValue[]
        {
            new FrequencyValue("Once", Frequency.Once),
            new FrequencyValue("Every Week", Frequency.EveryWeek),
            new FrequencyValue("Every Month", Frequency.EveryMonth),
            new FrequencyValue("Every Year", Frequency.EveryYear)
        };
        public class FrequencyValue
        {
            public FrequencyValue(string name, Frequency value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; set; }
            public Frequency Value { get; set; }
        }
    }
}
