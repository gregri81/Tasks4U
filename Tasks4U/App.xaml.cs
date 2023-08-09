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
using System.Configuration;
using System.Windows.Documents;
using System.Text.RegularExpressions;
using System.Windows.Markup;
using System.Collections.Generic;

namespace Tasks4U
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string UNIQUE_EVENT_NAME = "Tasks4UEvent_9b1ae09e-f034-40e3-b5b9-2214b6203bb4";
        private EventWaitHandle? _eventWaitHandle;
        private FileSystemWatcher _outlookFolderWatcher;

        public App() => SingleInstanceWatcher();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var tasks4UFolderPath = ConfigurationManager.AppSettings["sqliteFileDirectory"] ??
                                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tasks4U");

            Directory.CreateDirectory(tasks4UFolderPath);
            var dataSource = Path.Combine(tasks4UFolderPath, "tasks.db");

            var tasksContext = new TasksContext("Data Source=" + dataSource);

            var tasksViewModel = new TasksViewModel(tasksContext,
                                                 new MessageBoxService(),
                                                 new TasksListFlowDocumentGenerator(),
                                                 new TaskFlowDocumentGenerator(),
                                                 new TasksListWorksheetGenerator(),
                                                 new PdfService());

            new MainWindow() { DataContext = tasksViewModel }.Show();

            CreateOutlookFolderWatcher(tasksViewModel);
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

        // Monitors "FromOutlook" folder.
        // When an rtf file is created, shows a new task with subject and description read from this file.
        private void CreateOutlookFolderWatcher(TasksViewModel tasksViewModel)
        {
            // Outlook should write message to this directory and we should read them
            const string FromOutlookDir = "FromOutlook";

            Directory.CreateDirectory(FromOutlookDir);

            _outlookFolderWatcher = new FileSystemWatcher(FromOutlookDir, "task.rtf")
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                EnableRaisingEvents = true
            };            

            _outlookFolderWatcher.Renamed += (s, e) =>
            {
                if (e.Name != "task.rtf")
                    return;

                Current.Dispatcher.Invoke(() =>
                {
                    // Load the RTF file to a FlowDocument
                    var document = new FlowDocument();
                    var content = new TextRange(document.ContentStart, document.ContentEnd);

                    try
                    {
                        using var stream = new FileStream(e.FullPath, FileMode.Open);
                        content.Load(stream, DataFormats.Rtf);
                    }
                    catch (Exception)
                    {
                        // Don't do anything if unable to open the file
                        return;
                    }

                    // We read the file, now we can delete it
                    File.Delete(e.FullPath);

                    // Remove all blocks up to subject and extract subject
                    var subject = string.Empty;
                    var blocksToRemove = new List<Block>();

                    foreach (var block in document.Blocks)
                    {                        
                        blocksToRemove.Add(block);

                        var text = new TextRange(block.ContentStart, block.ContentEnd).Text;

                        var match = Regex.Match(text, @"Subject:(.*)", RegexOptions.None);

                        if (match.Success)
                        {
                            subject = match.Groups[1].Value.Trim();
                            break;
                        }
                    }

                    foreach (var block in blocksToRemove)
                        document.Blocks.Remove(block);

                    // Get the XAML representation of the email body
                    var description = XamlWriter.Save(document);

                    // Now use the extracted subject and body
                    tasksViewModel.ShowNewTask(subject, description);

                    if (Current.MainWindow is MainWindow mainWindow)
                        mainWindow.ShowTheWindow();
                });
            };
        }
    }
}
