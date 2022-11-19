using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Tasks4U.Models;
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

            new MainWindow()
            {
                DataContext = new TasksViewModel(tasksContext)
            }.Show();
        }
    }
}
