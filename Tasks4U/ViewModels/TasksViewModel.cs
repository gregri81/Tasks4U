using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Tasks4U.Models;

namespace Tasks4U.ViewModels
{
    public class TasksViewModel: ObservableObject
    {
        private TasksContext _tasksContext;

        public TasksViewModel(TasksContext tasksContext)
        {
            RemoveSelectedTasksCommand = new RelayCommand(RemoveSelectedTasks);
            ShowNewTaskCommand = new RelayCommand(ShowNewTask);
            AddTaskCommand = new RelayCommand(AddTask, () => !NewTaskViewModel.HasErrors);
            NewTaskViewModel.ErrorsChanged += (s, e) => AddTaskCommand.NotifyCanExecuteChanged();

            tasksContext.Database.EnsureCreated();
            _tasksContext = tasksContext;
            tasksContext.Tasks.Load();
        }

        public ICommand RemoveSelectedTasksCommand { get; }
        public ICommand ShowNewTaskCommand { get; }
        public RelayCommand AddTaskCommand { get; }

        public ObservableCollection<Task> Tasks => _tasksContext.Tasks.Local.ToObservableCollection();
        
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

        public TaskViewModel NewTaskViewModel { get; set; } = new TaskViewModel();
        private void AddTask()
        {
            var task = new Task(NewTaskViewModel.Name)
            {
                Description = NewTaskViewModel.Description,
                TaskFrequency = NewTaskViewModel.TaskFrequency,
                IntermmediateDate = NewTaskViewModel.IntermediateDate,
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
            IsNewTaskVisible = true;
            IsTasksListVisible = false;
        }

        private void ShowTasksList()
        {
            IsNewTaskVisible = false;
            IsTasksListVisible = true;
        }
    }
}
