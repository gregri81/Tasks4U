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

            var tasksContext = new TasksContext("Data Source=tasks.db");

            var notificationService = new NotificationService(tasksContext);

            new MainWindow(notificationService)
            {
                DataContext = new TasksViewModel(tasksContext, new Services.MessageBoxService())
            }.Show();

            notificationService.Start();
        }
    }
}
