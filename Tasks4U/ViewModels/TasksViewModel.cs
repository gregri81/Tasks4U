using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tasks4U.Models;
using Task = Tasks4U.Models.Task;

namespace Tasks4U.ViewModels
{
    public class TasksViewModel : ObservableObject
    {
        public event System.Action? BeforeSave;

        private TasksContext _tasksContext;

        public TasksViewModel(TasksContext tasksContext)
        {
            tasksContext.Database.EnsureCreated();
            _tasksContext = tasksContext;
            tasksContext.Tasks.Load();
            tasksContext.Desks.Load();

            RemoveSelectedTasksCommand = new RelayCommand(RemoveSelectedTasks);
            
            ShowNewTaskCommand = new RelayCommand(ShowNewTask);
            
            AddTaskCommand = new RelayCommand(AddTask, () => NewTaskViewModel.IsValid());
            
            NewTaskViewModel.IsValidChanged += () => AddTaskCommand.NotifyCanExecuteChanged();
            
            SaveCommand = new RelayCommand(Save, () => IsModifiedSinceLastSave);
            
            MarkAsModifiedIfNeededCommand = new RelayCommand(() => 
            { 
                if (_tasksContext.ChangeTracker.HasChanges()) 
                    IsModifiedSinceLastSave = true; 
            });

            Tasks = _tasksContext.Tasks.Local.ToObservableCollection();
            Tasks.CollectionChanged += (e, s) => IsModifiedSinceLastSave = true;

             Desks = _tasksContext.Desks.Local.ToObservableCollection();
        }

        #region properties

        public ICommand RemoveSelectedTasksCommand { get; }
        public ICommand ShowNewTaskCommand { get; }
        public RelayCommand AddTaskCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand MarkAsModifiedIfNeededCommand { get; }

        public ObservableCollection<Task> Tasks { get; }

        public ObservableCollection<Desk> Desks { get; }

        private bool _isNewTaskVisible = false;
        public bool IsNewTaskVisible
        {
            get => _isNewTaskVisible;
            set => SetProperty(ref _isNewTaskVisible, value);
        }

        private bool _isTasksListVisible = true;
        public bool IsTasksListVisible
        {
            get => _isTasksListVisible;
            set => SetProperty(ref _isTasksListVisible, value);
        }

        private string _saveButtonText = "Save";
        public string SaveButtonText
        {
            get => _saveButtonText;
            set => SetProperty(ref _saveButtonText, value);
        }

        private bool _isModifiedSinceLastSave;
        private bool IsModifiedSinceLastSave
        {
            get => _isModifiedSinceLastSave;
            set
            {
                _isModifiedSinceLastSave = value;
                SaveCommand.NotifyCanExecuteChanged();
            }
        }
        public TaskViewModel NewTaskViewModel { get; set; } = new TaskViewModel();

        #endregion

        #region methods
        private void AddTask()
        {
            var task = new Task(NewTaskViewModel.Name)
            {
                Description = NewTaskViewModel.Description,
                TaskFrequency = NewTaskViewModel.TaskFrequency,
                IntermediateDate = NewTaskViewModel.IntermediateDate,
                FinalDate = NewTaskViewModel.FinalDate
            };

            _tasksContext.Tasks.Add(task);

            ShowTasksList();
        }

        private void RemoveSelectedTasks()
        {
            var tasksToRemove = Tasks.Where(t => t.IsSelected).ToArray();

            foreach (var task in tasksToRemove)
                Tasks.Remove(task);
        }

        private void ShowNewTask()
        {
            NewTaskViewModel.Clear();
            IsNewTaskVisible = true;
            IsTasksListVisible = false;
        }

        private void ShowTasksList()
        {
            IsNewTaskVisible = false;
            IsTasksListVisible = true;
        }

        private void Save()
        {
            BeforeSave?.Invoke();

            // Don't save empty rows in desks datagrid
            var emptyDesks = _tasksContext.Desks.Local.Where(d => d.Name.Length == 0 && d.Description.Length == 0);
            _tasksContext.Desks.RemoveRange(emptyDesks);

            if (_tasksContext.Desks.Local.Any(d => d.Name.Length == 0))
            {
                MessageBox.Show("You cannot fill in description without name", "Empty name", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _tasksContext.SaveChanges();

            IsModifiedSinceLastSave = false;
        }

        
        #endregion
    }
}
