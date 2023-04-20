using CommunityToolkit.Mvvm.Input;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using Tasks4U.ViewModels;
using Tasks4U.Views;

namespace Tasks4U
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += (s, e) => DisableCloseButton();           
        }

        public void ShowTheWindow()
        {
            if (WindowState == WindowState.Minimized || Visibility != Visibility.Visible)
            {
                Show();
                WindowState = WindowState.Normal;
            }

            Activate();
            Topmost = true;
            Topmost = false;
            Focus();
        }       

        private void DisableCloseButton()
        {
            const int SC_CLOSE = 0xF060;
            const int MF_BYCOMMAND = 0x00000000;
            const int MF_GRAYED = 0x00000001;
            const int MF_DISABLED = 0x00000002;

            var handle = new WindowInteropHelper(this).Handle;

            EnableMenuItem(GetSystemMenu(handle, false), SC_CLOSE, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
        }

        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr menu, uint idEnableItem, uint enable);

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr handle, bool revert);

        private void LeftToRightButton_Click(object sender, RoutedEventArgs e)
        {
            NewTask.ChangeDirectionOfFocusedElement(Keyboard.FocusedElement, FlowDirection.LeftToRight);
        }

        private void RightToLeftButton_Click(object sender, RoutedEventArgs e)
        {
            NewTask.ChangeDirectionOfFocusedElement(Keyboard.FocusedElement, FlowDirection.RightToLeft);
        }


        // The events below call view-model commands.
        // We do it in the code-behind, because we need to read the Document property from the Description RichTextBox
        // and it's better to avoid passing RichTexBox to the view-model - we don't want GUI in our view-model.

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            var tasksViewModel = (TasksViewModel)DataContext;

            ExecuteCommandWithDescriptionArg(tasksViewModel.AddTaskCommand);          
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var tasksViewModel = (TasksViewModel)DataContext;

            ExecuteCommandWithDescriptionArg(tasksViewModel.ShowTasksCommand);
        }

        private void SaveAsPdfButton_Click(object sender, RoutedEventArgs e)
        {
            var tasksViewModel = (TasksViewModel)DataContext;

            if (tasksViewModel.IsNewTaskVisible)
                ExecuteCommandWithDescriptionArg(tasksViewModel.SaveTaskAsPdfCommand);
        }

        private void ExecuteCommandWithDescriptionArg(RelayCommand<FlowDocument> command)
        {
            var description = NewTask.Description.Document;

            if (command.CanExecute(description))
                command.Execute(description);
        }
    }
}
