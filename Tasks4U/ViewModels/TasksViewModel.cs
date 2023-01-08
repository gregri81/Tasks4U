using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tasks4U.Models;
using Tasks4U.Services;
using Task = Tasks4U.Models.Task;

namespace Tasks4U.ViewModels
{
    public class TasksViewModel : ObservableObject
    {
        private readonly TasksContext _tasksContext;
        private readonly IMessageBoxService _messageBoxService;
        private Task? _editedTask;

        public TasksViewModel(TasksContext tasksContext, IMessageBoxService messageBoxService)
        {
            _messageBoxService = messageBoxService;

            tasksContext.Database.EnsureCreated();
            _tasksContext = tasksContext;
            tasksContext.Tasks.Load();

            RemoveSelectedTasksCommand = new RelayCommand(RemoveSelectedTasks, () => Tasks != null && Tasks.Any(t => t.IsSelected));
            
            ShowNewTaskCommand = new RelayCommand(ShowNewTask);

            EditTaskCommand = new RelayCommand(EditTask, () => Tasks != null && Tasks.Where(t => t.IsSelected).Take(2).Count() == 1);

            ShowTasksCommand = new RelayCommand(ShowTasksListWithoutSaving);

            AddTaskCommand = new RelayCommand(AddTask, () => NewTaskViewModel.IsValid());
            
            NewTaskViewModel.IsValidChanged += () => AddTaskCommand.NotifyCanExecuteChanged();
            
            SaveCommand = new RelayCommand(Save, () => IsModifiedSinceLastSave);

            HandleWindowClosingCommand = new RelayCommand(HandleWindowClosing);

            Tasks = _tasksContext.Tasks.Local.ToObservableCollection();
            
            RegisterCallbackHandlersForTasksCollection();
        }

        #region commands

        public RelayCommand RemoveSelectedTasksCommand { get; }
        public ICommand ShowNewTaskCommand { get; }

        public RelayCommand EditTaskCommand { get; }

        public ICommand ShowTasksCommand { get; }
        public RelayCommand AddTaskCommand { get; }
        public RelayCommand SaveCommand { get; }

        public ICommand HandleWindowClosingCommand { get; }

        #endregion

        #region properties

        public FilterViewModel Filter { get; } = new FilterViewModel();

        public ObservableCollection<Task> Tasks { get; }

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
        private void RegisterCallbackHandlersForTasksCollection()
        {
            var OnIsSelectedChanged = () =>
            {
                RemoveSelectedTasksCommand.NotifyCanExecuteChanged();
                EditTaskCommand.NotifyCanExecuteChanged();
            };

            foreach (Task task in Tasks)
                task.IsSelectedChanged += OnIsSelectedChanged;

            Tasks.CollectionChanged += (e, s) =>
            {
                IsModifiedSinceLastSave = true;

                RemoveSelectedTasksCommand.NotifyCanExecuteChanged();
                EditTaskCommand.NotifyCanExecuteChanged();

                if (s.NewItems != null)
                {
                    foreach (Task task in s.NewItems)
                        task.IsSelectedChanged += OnIsSelectedChanged;
                }
            };
        }

        private void AddTask()
        {
            var task = new Task(NewTaskViewModel.Name)
            {
                Description = NewTaskViewModel.Description,
                TaskFrequency = NewTaskViewModel.TaskFrequency,
                RelatedTo = NewTaskViewModel.RelatedTo,
                Desk = NewTaskViewModel.Desk,
                IntermediateDate = NewTaskViewModel.IntermediateDate,
                FinalDate = NewTaskViewModel.FinalDate,
                Status = NewTaskViewModel.Status
            };

            _tasksContext.Tasks.Add(task);

            if (_editedTask != null)
            {
                _tasksContext.Remove(_editedTask);
                _editedTask = null;
            }

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

        private void EditTask()
        {
            _editedTask = Tasks.Single(t => t.IsSelected);

            NewTaskViewModel.Clear();

            NewTaskViewModel.Name = _editedTask.Name;
            NewTaskViewModel.Description = _editedTask.Description;            
            NewTaskViewModel.RelatedTo = _editedTask.RelatedTo;
            NewTaskViewModel.Desk = _editedTask.Desk;
            NewTaskViewModel.Status = _editedTask.Status;

            NewTaskViewModel.DisableDateValidation(true);
            NewTaskViewModel.TaskFrequency = _editedTask.TaskFrequency;
            NewTaskViewModel.IntermediateDate = _editedTask.IntermediateDate;
            NewTaskViewModel.FinalDate = _editedTask.FinalDate;
            NewTaskViewModel.DisableDateValidation(false);

            IsNewTaskVisible = true;
            IsTasksListVisible = false;
        }

        private void ShowTasksListWithoutSaving()
        {

            if (_messageBoxService.Show("Are you sure that you want to cancel?", "Any changes will be unsaved",
                                        MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ShowTasksList();
            }
        }

        private void ShowTasksList()
        {
            IsNewTaskVisible = false;
            IsTasksListVisible = true;
        }

        private void HandleWindowClosing()
        {
            if (IsModifiedSinceLastSave &&
                _messageBoxService.Show("Do you want to save your changes", "There are unsaved changes",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Save();
            }
        }

        private void Save()
        {
            try
            {
                _tasksContext.SaveChanges();
            }
            catch(DbUpdateException exception) when (exception.InnerException != null)
            {
                if (exception.InnerException.Message.EndsWith("'UNIQUE constraint failed: Tasks.Name'."))
                    ShowSubjectNotUniqueMessageBox(exception);
            }

            IsModifiedSinceLastSave = false;
        }

        private void ShowSubjectNotUniqueMessageBox(DbUpdateException exception)
        {
            var message = "You cannot use the same subject twice.";

            if (exception.Entries.Count > 0 && exception.Entries[0].Entity is Task task)
                message += " Subject: " + task.Name;

            _messageBoxService.Show(message, "Subject must be unique", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        
        #endregion
    }
}
