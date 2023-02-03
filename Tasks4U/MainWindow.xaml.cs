using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Tasks4U.Services;
using Tasks4U.ViewModels;

namespace Tasks4U
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(NotificationService notificationService)
        {
            InitializeComponent();

            Loaded += (s, e) => DisableCloseButton();

            DataContextChanged += (s, e) =>
            {
                var tasksViewModel = (TasksViewModel)DataContext;
                notificationService.NotificationClick += taskId => tasksViewModel.EditSpecifiedTask(taskId);
            };           
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
    }
}
