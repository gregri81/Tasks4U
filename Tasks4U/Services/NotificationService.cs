using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Tasks4U.Services
{
    public interface INotificationService
    {
        void ShowNotification(string title, string text);
    }

    public class NotificationService: INotificationService
    {
        public void ShowNotification(string title, string text)
        {
            var notificationIcon = new NotifyIcon();

            var iconUri = new Uri("pack://application:,,,/Images/tasks.ico");
            var iconStream = System.Windows.Application.GetResourceStream(iconUri).Stream;

            notificationIcon.Icon = new Icon(iconStream);
            notificationIcon.Icon = SystemIcons.Asterisk;
            notificationIcon.Visible = true;
            notificationIcon.ShowBalloonTip(5000, title, text, ToolTipIcon.Error);

            notificationIcon.Dispose();
        }
    }
}
