using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks4U.Models;
using TaskStatus = Tasks4U.Models.TaskStatus;

namespace Tasks4U.ViewModels
{
    public class FilterViewModel: ObservableObject
    {
        public static Array FilterValues => Enum.GetValues(typeof(FilterType));
        public static Array FrequencyValues => Enum.GetValues(typeof(Frequency));

        public static Array StatusValues => Enum.GetValues(typeof(Models.TaskStatus));

        public static Array DeskValues => Enum.GetValues(typeof(Desk));

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
            }
        }
        public bool IsTextFilter => 
            _selectedFilter == FilterType.Subject || _selectedFilter == FilterType.RelatedTo || _selectedFilter == FilterType.Description;

        public bool IsDeskFilter => _selectedFilter == FilterType.Desk;

        public bool IsStatusFilter => _selectedFilter == FilterType.Status;

        public bool IsDateFilter => _selectedFilter == FilterType.Date;

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

        public enum FilterType { None, Subject, Frequency, Date, RelatedTo, Desk, Status, Description }
    }
}
