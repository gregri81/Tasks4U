using System;
using System.IO;
using System.Windows;
using Tasks4U.Models;
using Tasks4U.Services;
using Tasks4U.ViewModels;

namespace Tasks4U
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var dataSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "tasks.db");

            var tasksContext = new TasksContext("Data Source=" + dataSource);

            var notificationService = new NotificationService(tasksContext);

            new MainWindow(notificationService)
            {
                DataContext = new TasksViewModel(tasksContext, new Services.MessageBoxService())
            }.Show();

            notificationService.Start();
        }
    }
}
