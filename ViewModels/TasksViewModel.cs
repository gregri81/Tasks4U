using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;
using Tasks4U.Models;

namespace Tasks4U.ViewModels
{
    public class TasksViewModel: ObservableObject
    {
        private TasksContext _tasksContext;

        public TasksViewModel(TasksContext tasksContext)
        {
            AddTaskCommand = new RelayCommand(AddTask);
            RemoveTaskCommand = new RelayCommand(RemoveTask);
            ShowNewTaskCommand = new RelayCommand(ShowNewTask);

            tasksContext.Database.EnsureCreated();
            _tasksContext = tasksContext;
            tasksContext.Tasks.Load();
        }

        public ICommand AddTaskCommand { get; }
        public ICommand RemoveTaskCommand { get; }
        public ICommand ShowNewTaskCommand { get; }

        public IEnumerable<Task> Tasks => _tasksContext.Tasks.Local.ToObservableCollection();
        
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

        private void RemoveTask()
        {
            
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
