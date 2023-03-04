using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Tasks4U.ViewModels;

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

            Grid.DataContextChanged += (s, e) =>
            {
                var dataContext = ((Grid)s).DataContext;
                var taskViewModel = ((TaskViewModel)dataContext);

                taskViewModel.PropertyChanged += (sender, ev) =>
                {
                    if (ev.PropertyName == nameof(taskViewModel.Description))
                        LoadDescription(taskViewModel.Description);
                };
            };
        }

        public RichTextBox Description => DescriptionRichTextBox;

        private void LoadDescription(string descriptionXaml)
        {
            try
            {
                DescriptionRichTextBox.Document = (FlowDocument)XamlReader.Parse(descriptionXaml);
            }
            catch (System.Windows.Markup.XamlParseException)
            {
                // If we cannot parse descriptionXaml as XAML, treat it as plain text
                DescriptionRichTextBox.Document.Blocks.Clear();
                DescriptionRichTextBox.AppendText(descriptionXaml);
            }
        }
    }
}
