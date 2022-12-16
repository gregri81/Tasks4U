using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Tasks4U.Views
{
    /// <summary>
    /// Interaction logic for NewTask.xaml
    /// </summary>
    public partial class NewTask : UserControl
    {
        public NewTask()
        {
            InitializeComponent();

            // Whe the UserControl becomes visible, set focus on Name TextBox when
            IsVisibleChanged += (s, e) =>
            {
                if (Visibility == Visibility.Visible)
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        FocusManager.SetFocusedElement(this, NameTextBox);
                        Keyboard.Focus(NameTextBox);
                    }, DispatcherPriority.Render);
                }                
            };
        }
    }
}
