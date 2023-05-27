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
using System.Windows.Markup;
using System.Windows.Threading;
using Tasks4U.Models;
using Tasks4U.Services;
using Tasks4U.FlowDocumentGenerators;
using ClosedXML.Excel;
using System.Data;

using Task = Tasks4U.Models.Task;
using FlowDirection = System.Windows.FlowDirection;
using System.Windows.Documents;

namespace Tasks4U.ViewModels
{
    public class TasksViewModel : ObservableObject
    {
        // The customer's request is to start receiving notifications at 9 o`clock
        private readonly TimeSpan _notificationsStartTime = TimeSpan.FromHours(9);

        // The customer's request is to stop receiving notifications at 5 o`clock pm
        private readonly TimeSpan _notificationsEndTime = TimeSpan.FromHours(17);

        private readonly TasksContext _tasksContext;
        private readonly IMessageBoxService _messageBoxService;
        private readonly DispatcherTimer _timer;

        private Task? _editedTask;
        private TaskViewModel.Memento? _taskMementoBeforeEditing;
        private DateOnly _currentDate = DateOnly.FromDateTime(DateTime.Now);
        private TasksListFlowDocumentGenerator _tasksListDocumentGenerator;
        private TaskFlowDocumentGenerator _taskDocumentGenerator;
        private TasksListWorksheetGenerator _tasksListWorksheetGenerator;
        private IPdfService _pdfService;

        private Dictionary<NotifyIcon, DateTime> _notificationIcons = new Dictionary<NotifyIcon, DateTime>();

        public TasksViewModel(
            TasksContext tasksContext,
            IMessageBoxService messageBoxService,
            TasksListFlowDocumentGenerator tasksListDocumentGenerator,
            TaskFlowDocumentGenerator taskDocumentGenerator,
            TasksListWorksheetGenerator tasksListWorksheetGenerator,
            IPdfService pdfService)
        {
            _messageBoxService = messageBoxService;
            _tasksListDocumentGenerator = tasksListDocumentGenerator;
            _taskDocumentGenerator = taskDocumentGenerator;
            _tasksListWorksheetGenerator = tasksListWorksheetGenerator;
            _pdfService = pdfService;

            tasksContext.Database.EnsureCreated();
            _tasksContext = tasksContext;
            tasksContext.Tasks.Load();

            RemoveSelectedTasksCommand = new RelayCommand(RemoveSelectedTasks, () => Tasks != null && Tasks.Any(t => t.IsSelected));

            ShowNewTaskCommand = new RelayCommand(ShowNewTask);

            EditSelectedTaskCommand = new RelayCommand(EditSelectedTask, () => Tasks != null && Tasks.Where(t => t.IsSelected).Take(2).Count() == 1);

            EditTaskCommand = new RelayCommand<Task?>(task => EditTask(task));

            ShowTasksCommand = new RelayCommand<FlowDocument>(description => ShowTasksListWithoutSaving(description));

            AddTaskCommand = new RelayCommand<FlowDocument>(description => AddTask(description), (description) => NewTaskViewModel.IsValid());

            SaveTasksListAsPdfCommand = new RelayCommand<IEnumerable<Task>>(tasks => SaveTasksListAsPdf(tasks));

            SaveTaskAsPdfCommand = new RelayCommand<FlowDocument>(description => SaveTaskAsPdf(description));

            SaveTasksListAsExcelCommand = new RelayCommand<IEnumerable<Task>>(tasks => SaveTasksListAsExcel(tasks));

            NewTaskViewModel.IsValidChanged += () =>
            {
                AddTaskCommand.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(AddTaskCommandCanExecute));
            };

            Tasks = _tasksContext.Tasks.Local.ToObservableCollection();

            RegisterCallbackHandlersForTasksCollection();
            CheckForOutdatedTasks();

            // Start a timer that renews recurring tasks,
            // shows notifications and sorts the tasks datagrid when the date changes
            _timer = new DispatcherTimer() { Interval = TimeSpan.FromMinutes(1) };
            _timer.Tick += (s, e) => TimerCallback();
            _timer.Start();
        }

        public event Action? IsDateChanged;

        #region commands

        public RelayCommand RemoveSelectedTasksCommand { get; }
        public ICommand ShowNewTaskCommand { get; }

        public RelayCommand EditSelectedTaskCommand { get; }
        public RelayCommand<Task?> EditTaskCommand { get; }

        public RelayCommand<FlowDocument> ShowTasksCommand { get; }
        public RelayCommand<FlowDocument> AddTaskCommand { get; }

        public RelayCommand<IEnumerable<Task>> SaveTasksListAsPdfCommand { get; }
        public RelayCommand<FlowDocument> SaveTaskAsPdfCommand { get; }

        public RelayCommand<IEnumerable<Task>> SaveTasksListAsExcelCommand { get; }

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

        private bool _isKeyboardFocusOnTextBox;
        public bool IsKeyboardFocusOnTextBox
        {
            get => _isKeyboardFocusOnTextBox;
            set => SetProperty(ref _isKeyboardFocusOnTextBox, value);
        }

        private bool _isKeyboardFocusOnDescription;
        public bool IsKeyboardFocusOnDescription
        {
            get => _isKeyboardFocusOnDescription;
            set => SetProperty(ref _isKeyboardFocusOnDescription, value);
        }

        public bool AddTaskCommandCanExecute => AddTaskCommand.CanExecute(null);

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
                task.IsSelectedChanged += OnIsSelectedChanged;

            Tasks.CollectionChanged += (e, s) =>
            {
                RemoveSelectedTasksCommand.NotifyCanExecuteChanged();
                EditSelectedTaskCommand.NotifyCanExecuteChanged();

                if (s.NewItems != null)
                {
                    var today = DateOnly.FromDateTime(DateTime.Today);

                    foreach (Task task in s.NewItems)
                    {
                        task.IsSelectedChanged += OnIsSelectedChanged;
                        task.IsOutdated = task.FinalDate <= today && task.Status != TaskStatus.Finished;
                    }
                }
            };
        }

        private void CheckForOutdatedTasks()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            foreach (Task task in Tasks)
                task.IsOutdated = task.FinalDate <= today && task.Status != TaskStatus.Finished;
        }

        private void AddTask(FlowDocument? description)
        {
            var task = new Task(NewTaskViewModel.Name)
            {
                Description = description == null ? string.Empty : XamlWriter.Save(description),
                TaskFrequency = NewTaskViewModel.TaskFrequency,
                RelatedTo = NewTaskViewModel.RelatedTo,
                Desk = NewTaskViewModel.Desk,
                IntermediateDate = NewTaskViewModel.IntermediateDate,
                FinalDate = NewTaskViewModel.FinalDate,
                Status = NewTaskViewModel.Status,
                IsNameLeftToRight = NewTaskViewModel.NameDirection == FlowDirection.LeftToRight,
                IsRelatedToLeftToRight = NewTaskViewModel.RelatedToDirection == FlowDirection.LeftToRight,
                IsDescriptionLeftToRight = NewTaskViewModel.DescriptionDirection == FlowDirection.LeftToRight,
            };

            // The tasks may be saved in the timer callback,
            // but adding this task might cause an error,
            // so we should temporarily disable the timer
            _timer.IsEnabled = false;

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

            _timer.IsEnabled = true;

            _editedTask = null;
            _taskMementoBeforeEditing = null;
        }

        private void SaveTasksListAsPdf(IEnumerable<Task>? tasks)
        {
            if (tasks != null)
                _pdfService.SaveAsPdf(_tasksListDocumentGenerator.Generate(tasks), _messageBoxService);
        }

        private void SaveTaskAsPdf(FlowDocument? description)
        {
            if (description != null)
            {
                var flowDocument = _taskDocumentGenerator.Generate(NewTaskViewModel, description);
                _pdfService.SaveAsPdf(flowDocument, _messageBoxService);
            }
        }

        private void SaveTasksListAsExcel(IEnumerable<Task>? tasks)
        {
            if (tasks == null)
                return;

            var saveFileDialog = new SaveFileDialog() { Filter = "*.xlsx|*.xlsx" };

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            using (var excelWorkbook = new XLWorkbook())
            {
                _tasksListWorksheetGenerator.Generate(excelWorkbook, tasks);

                try
                {
                    excelWorkbook.SaveAs(saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    _messageBoxService.Show($"Failed to save tasks to '{saveFileDialog.FileName}'. {Environment.NewLine} {ex.Message}",
                                            "Cannot Save",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Error);
                }
            }
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
            NewTaskViewModel.NameDirection = GetDirection(task.IsNameLeftToRight);
            NewTaskViewModel.Description = _editedTask.Description;
            NewTaskViewModel.DescriptionDirection = GetDirection(task.IsDescriptionLeftToRight);
            NewTaskViewModel.RelatedTo = _editedTask.RelatedTo;
            NewTaskViewModel.RelatedToDirection = GetDirection(task.IsRelatedToLeftToRight);
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

        private FlowDirection GetDirection(bool isLeftToRight) => 
            isLeftToRight ? FlowDirection.LeftToRight: FlowDirection.RightToLeft;

        private void ShowTasksListWithoutSaving(FlowDocument? description)
        {
            if (description != null)
                NewTaskViewModel.Description = XamlWriter.Save(description);

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
            RemoveTimedOutNotificationIcons();
            RenewRecurringTasks();

            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);

            // This is needed in order to re-sort the tasks datagrid when the date changes
            if (today > _currentDate)
            {
                _currentDate = today;
                IsDateChanged?.Invoke();
                CheckForOutdatedTasks();
            }

            if (now.TimeOfDay < _notificationsStartTime || now.TimeOfDay > _notificationsEndTime)
                return;

            var finalDateCorrespondingTasks =
                _tasksContext.Tasks.AsEnumerable()
                                   .Where(task => task.ShouldShowFinalNotification(now))
                                   .ToList();

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
                _notificationIcons.Remove(notificationIcon);
                EditTask(task);
            };

            notificationIcon.Click += (s, e) =>
            {
                notificationIcon.Dispose();
                _notificationIcons.Remove(notificationIcon);
                EditTask(task);
            };
            
            _notificationIcons.Add(notificationIcon, DateTime.Now);

            notificationIcon.ShowBalloonTip(
                30000, task.Name + taskFrequencyDescription, text, ToolTipIcon.Warning);
        }

        private void RemoveTimedOutNotificationIcons()
        {
            var now = DateTime.Now;

            var timedOutNotificationIcons = _notificationIcons.Where(pair => (now - pair.Value).TotalMinutes >= 60)
                                                              .Select(pair => pair.Key)
                                                              .ToArray();

            foreach (var icon in timedOutNotificationIcons)
            {
                icon.Dispose();
                _notificationIcons.Remove(icon);
            }
        }

        private void RenewRecurringTasks()
        {
            var renewedTasks = new List<int>();

            foreach (var task in _tasksContext.Tasks)
            {
                if (task.RenewRecurringTaskIfNeeded())
                    renewedTasks.Add(task.ID);
            }

            if (renewedTasks.Count > 0)
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
