using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;
using Tasks4U.Models;

namespace Tasks4U.Services
{
    public interface INotificationService
    {
        void ShowNotification(Task task, string text);
    }

    public class NotificationService: INotificationService
    {
        // The customer's request is to start receiving notification at 9 o`clock
        private readonly TimeSpan _notificationsStartTime = TimeSpan.FromHours(9);

        private readonly TasksContext _tasksContext;
        public NotificationService(TasksContext tasksContext) => _tasksContext = tasksContext;

        public event Action<int /* task id*/>? NotificationClick;

        public void Start()
        {
            var timer = new DispatcherTimer() { Interval = TimeSpan.FromMinutes(1) };
            timer.Tick += (s, e) => TimerCallback();
            timer.Start();
        }

        public void ShowNotification(Task task, string text)
        {
            var notificationIcon = new NotifyIcon();

            var iconUri = new Uri("pack://application:,,,/Images/tasks.ico");
            var iconStream = System.Windows.Application.GetResourceStream(iconUri).Stream;

            notificationIcon.Icon = new Icon(iconStream);
            notificationIcon.Icon = SystemIcons.Asterisk;
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
                NotificationClick?.Invoke(task.ID);
            };

            notificationIcon.BalloonTipClosed += (s, e) => notificationIcon.Dispose();

            notificationIcon.ShowBalloonTip(
                5000, task.Name + taskFrequencyDescription, text, ToolTipIcon.Error);            
        }


        private void TimerCallback()
        {
            var now = DateTime.Now;

            if (now.TimeOfDay >= _notificationsStartTime)
            {
                var today = DateOnly.FromDateTime(now);

                var intermediateDateCorrespondingTasks =
                    _tasksContext.Tasks.AsEnumerable()
                                       .Where(task => task.ShouldShowIntermediateNotification(today))
                                       .ToList();

                var finalDateCorrespondingTasks =
                    _tasksContext.Tasks.AsEnumerable()
                                       .Where(task => task.ShouldShowFinalNotification(today))
                                       .ToList();

                foreach (var task in intermediateDateCorrespondingTasks)
                {
                    ShowNotification(task, "Intermediate date is today");
                    task.LastIntermediateNotificationDate = today;
                }

                foreach (var task in finalDateCorrespondingTasks)
                {
                    ShowNotification(task, "Final date is today");
                    task.LastFinalNotificationDate = today;
                }
            }
        }
    }
}
