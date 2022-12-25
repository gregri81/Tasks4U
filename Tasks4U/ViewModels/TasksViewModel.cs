using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
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
        private readonly TasksContext _tasksContext;

        public TasksViewModel(TasksContext tasksContext)
        {
            tasksContext.Database.EnsureCreated();
            _tasksContext = tasksContext;
            tasksContext.Tasks.Load();

            RemoveSelectedTasksCommand = new RelayCommand(RemoveSelectedTasks);
            
            ShowNewTaskCommand = new RelayCommand(ShowNewTask);
            
            AddTaskCommand = new RelayCommand(AddTask, () => NewTaskViewModel.IsValid());
            
            NewTaskViewModel.IsValidChanged += () => AddTaskCommand.NotifyCanExecuteChanged();
            
            SaveCommand = new RelayCommand(Save, () => IsModifiedSinceLastSave);
            
            Tasks = _tasksContext.Tasks.Local.ToObservableCollection();
            Tasks.CollectionChanged += (e, s) => IsModifiedSinceLastSave = true;
        }

        #region properties

        public ICommand RemoveSelectedTasksCommand { get; }
        public ICommand ShowNewTaskCommand { get; }
        public RelayCommand AddTaskCommand { get; }
        public RelayCommand SaveCommand { get; }

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
        private void AddTask()
        {
            var task = new Task(NewTaskViewModel.Name)
            {
                Description = NewTaskViewModel.Description,
                TaskFrequency = NewTaskViewModel.TaskFrequency,
                RelatedTo = NewTaskViewModel.RelatedTo,
                Desk = NewTaskViewModel.Desk,
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

            MessageBox.Show(message, "Subject must be unique", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        
        #endregion
    }
}
