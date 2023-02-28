using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using Tasks4U.Models;
using Tasks4U.Services;
using Task = Tasks4U.Models.Task;

namespace Tasks4U.ViewModels
{
    public class TasksViewModel : ObservableObject
    {
        // The customer's request is to start receiving notification at 9 o`clock
        private readonly TimeSpan _notificationsStartTime = TimeSpan.FromHours(9);

        // The customer's request is to stop receiving notification at 5 o`clock pm
        private readonly TimeSpan _notificationsEndTime = TimeSpan.FromHours(17);

        private readonly TasksContext _tasksContext;
        private readonly IMessageBoxService _messageBoxService;
        
        private Task? _editedTask;
        private TaskViewModel.Memento? _taskMementoBeforeEditing;
        private DateOnly _currentDate = DateOnly.FromDateTime(DateTime.Now);

        public TasksViewModel(
            TasksContext tasksContext, 
            IMessageBoxService messageBoxService)
        {
            _messageBoxService = messageBoxService;

            tasksContext.Database.EnsureCreated();
            _tasksContext = tasksContext;
            tasksContext.Tasks.Load();

            RemoveSelectedTasksCommand = new RelayCommand(RemoveSelectedTasks, () => Tasks != null && Tasks.Any(t => t.IsSelected));

            ShowNewTaskCommand = new RelayCommand(ShowNewTask);

            EditSelectedTaskCommand = new RelayCommand(EditSelectedTask, () => Tasks != null && Tasks.Where(t => t.IsSelected).Take(2).Count() == 1);

            EditTaskCommand = new RelayCommand<Task?>(task => EditTask(task));

            ShowTasksCommand = new RelayCommand(ShowTasksListWithoutSaving);

            AddTaskCommand = new RelayCommand(AddTask, () => NewTaskViewModel.IsValid());

            NewTaskViewModel.IsValidChanged += () => AddTaskCommand.NotifyCanExecuteChanged();

            Tasks = _tasksContext.Tasks.Local.ToObservableCollection();

            RegisterCallbackHandlersForTasksCollection();

            // Start a timer that renews recurring tasks,
            // shows notifications and sorts the tasks datagrid when the date changes
            var timer = new DispatcherTimer() { Interval = TimeSpan.FromMinutes(1) };
            timer.Tick += (s, e) => TimerCallback();
            timer.Start();
        }

        public event Action? IsDateChanged;

        #region commands

        public RelayCommand RemoveSelectedTasksCommand { get; }
        public ICommand ShowNewTaskCommand { get; }

        public RelayCommand EditSelectedTaskCommand { get; }
        public RelayCommand<Task?> EditTaskCommand { get; }

        public ICommand ShowTasksCommand { get; }
        public RelayCommand AddTaskCommand { get; }

        #endregion

        #region properties

        public FilterViewModel Filter { get; } = new FilterViewModel();

        public ObservableCollection<Task> Tasks { get; set; }

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

        #endregion

        #region methods                

        private void RegisterCallbackHandlersForTasksCollection()
        {
            var OnIsSelectedChanged = () =>
            {
                RemoveSelectedTasksCommand.NotifyCanExecuteChanged();
                EditSelectedTaskCommand.NotifyCanExecuteChanged();
            };

            foreach (Task task in Tasks)
                task.IsUnmappedRowPropertyChanged += OnIsSelectedChanged;

            Tasks.CollectionChanged += (e, s) =>
            {
                RemoveSelectedTasksCommand.NotifyCanExecuteChanged();
                EditSelectedTaskCommand.NotifyCanExecuteChanged();

                if (s.NewItems != null)
                {
                    foreach (Task task in s.NewItems)
                        task.IsUnmappedRowPropertyChanged += OnIsSelectedChanged;
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
                _tasksContext.Remove(_editedTask);

            if (Save())
            {
                ShowTasksList();
            }
            else
            {
                // In case of failure, undo the last change
                _tasksContext.Tasks.Remove(task);

                if (_editedTask != null)
                    _tasksContext.Add(_editedTask);
            }

            _editedTask = null;
            _taskMementoBeforeEditing = null;
        }

        private void RemoveSelectedTasks()
        {
            var tasksToRemove = Tasks.Where(t => t.IsSelected).ToArray();

            foreach (var task in tasksToRemove)
                Tasks.Remove(task);

            Save();
        }

        private void ShowNewTask()
        {
            NewTaskViewModel.Clear();
            IsNewTaskVisible = true;
            IsTasksListVisible = false;
        }

        private void EditSelectedTask() => EditTask(Tasks.SingleOrDefault(t => t.IsSelected));

        private void EditTask(Task? task)
        {
            if (task == null)
                return;

            _editedTask = task;

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

            _taskMementoBeforeEditing = NewTaskViewModel.CreateMemento();

            IsNewTaskVisible = true;
            IsTasksListVisible = false;
        }

        private void ShowTasksListWithoutSaving()
        {
            if (_taskMementoBeforeEditing == NewTaskViewModel.CreateMemento())
            {
                ShowTasksList();
            }
            else if (_messageBoxService.Show("Are you sure that you want to cancel?", "Any changes will be unsaved",
                                             MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ShowTasksList();
            }
        }

        private void ShowTasksList()
        {
            _editedTask = null;
            _taskMementoBeforeEditing = null;
            IsNewTaskVisible = false;
            IsTasksListVisible = true;
        }        

        private bool Save()
        {
            try
            {
                _tasksContext.SaveChanges();
            }
            catch (DbUpdateException exception) when (exception.InnerException != null)
            {
                if (exception.InnerException.Message.EndsWith("'UNIQUE constraint failed: Tasks.Name'."))
                {
                    _messageBoxService.Show("You cannot use the same subject twice.", 
                                            "Subject must be unique", 
                                            MessageBoxButton.OK, 
                                            MessageBoxImage.Warning);
                }
                else
                {
                    _messageBoxService.Show(exception.InnerException.Message, 
                                            "Failed to save", 
                                            MessageBoxButton.OK, 
                                            MessageBoxImage.Warning);
                }

                return false;
            }

            return true;
        }

        #endregion

        #region methods that are called by timer

        private void TimerCallback()
        {
            RenewRecurringTasks();

            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);

            // This is needed in order to re-sort the tasks datagrid when the date changes
            if (today > _currentDate)
            {
                _currentDate = today;
                IsDateChanged?.Invoke();
            }

            if (now.TimeOfDay < _notificationsStartTime || now.TimeOfDay > _notificationsEndTime)
                return;

            var intermediateDateCorrespondingTasks =
                _tasksContext.Tasks.AsEnumerable()
                                   .Where(task => task.ShouldShowIntermediateNotification(now))
                                   .ToList();

            var finalDateCorrespondingTasks =
                _tasksContext.Tasks.AsEnumerable()
                                   .Where(task => task.ShouldShowFinalNotification(now))
                                   .ToList();

            foreach (var task in intermediateDateCorrespondingTasks)
            {
                ShowNotification(task, "Intermediate date is today");
                task.LastNotificationTime = now;
            }

            foreach (var task in finalDateCorrespondingTasks)
            {
                var message = task.IsDayOfFinalDate(today)
                              ? "Final date is today"
                              : "Final date has passed";

                ShowNotification(task, message);
                task.LastNotificationTime = now;
            }
        }

        private void ShowNotification(Task task, string text)
        {
            var notificationIcon = new NotifyIcon();

            var iconUri = new Uri("pack://application:,,,/Images/tasks.ico");
            var iconStream = System.Windows.Application.GetResourceStream(iconUri).Stream;

            notificationIcon.Icon = new Icon(iconStream);
            notificationIcon.Visible = true;

            string taskFrequencyDescription = task.TaskFrequency switch
            {
                Frequency.EveryWeek => " (Weekly)",
                Frequency.EveryMonth => " (Monthly)",
                Frequency.EveryYear => " (Yearly)",
                _ => string.Empty
            };

            notificationIcon.BalloonTipClicked += (s, e) =>
            {
                notificationIcon.Dispose();
                EditTask(task);
            };

            notificationIcon.BalloonTipClosed += (s, e) =>
            {
                notificationIcon.Dispose();
            };

            notificationIcon.ShowBalloonTip(
                5000, task.Name + taskFrequencyDescription, text, ToolTipIcon.Warning);
        }

        private void RenewRecurringTasks()
        {
            var renewedTasks = new List<int>();

            foreach (var task in _tasksContext.Tasks)
            {
                if (task.RenewRecurringTaskIfNeeded())
                    renewedTasks.Add(task.ID);
            }

            _tasksContext.SaveChanges();

            foreach (var taskId in renewedTasks)
            {
                var task = Tasks.FirstOrDefault(t => t.ID == taskId);
                task?.ClearStatus();
            }
        }
        
        #endregion
    }
}
