using System;
using System.IO;
using System.Threading;
using System.Windows;
using Tasks4U.Services;
using Tasks4U.ViewModels;
using Tasks4U.Models;
using Tasks4U.FlowDocumentGenerators;
using System.Globalization;
using System.Windows.Input;

namespace Tasks4U
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string UNIQUE_EVENT_NAME = "Tasks4UEvent_9b1ae09e-f034-40e3-b5b9-2214b6203bb4";
        private EventWaitHandle? _eventWaitHandle;

        public App() => SingleInstanceWatcher();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var dataSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "tasks.db");

            var tasksContext = new TasksContext("Data Source=" + dataSource);            

            new MainWindow()
            {
                DataContext = new TasksViewModel(tasksContext,
                                                 new MessageBoxService(),
                                                 new TasksListFlowDocumentGenerator(),
                                                 new TaskFlowDocumentGenerator(),
                                                 new TasksListWorksheetGenerator(),
                                                 new PdfService())
            }.Show();            
        }

        // If an instance of this application already exits, this code shows the existing instance. Otherwise, it runs a new instance.
        private void SingleInstanceWatcher()
        {
            try
            {
                // try to open the event - we will succeed if another instance is running
                _eventWaitHandle = EventWaitHandle.OpenExisting(UNIQUE_EVENT_NAME);
                _eventWaitHandle.Set();
                Shutdown();
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                // We are the "master" instance - create the event
                _eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, UNIQUE_EVENT_NAME);
            }

            new System.Threading.Tasks.Task(() =>
            {
                while (_eventWaitHandle.WaitOne())
                {
                    Current.Dispatcher.BeginInvoke(() =>
                    {
                        if (Current.MainWindow is MainWindow mainWindow)
                            mainWindow.ShowTheWindow();
                    });
                }
            }).Start();
        }
    }
}
