using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using Tasks4U.FlowDocumentGenerators;
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

            DockPanel.DataContextChanged += (s, e) =>
            {
                var dataContext = ((DockPanel)s).DataContext;
                var taskViewModel = ((TaskViewModel)dataContext);

                taskViewModel.PropertyChanged += (sender, ev) =>
                {
                    if (ev.PropertyName == nameof(taskViewModel.Description))
                        LoadDescription(taskViewModel.Description);
                };
            };

            EventManager.RegisterClassHandler(
               typeof(UIElement),
               Keyboard.GotKeyboardFocusEvent,
               new RoutedEventHandler(OnKeyboardFocus), true);
        }

        public RichTextBox Description => DescriptionRichTextBox;

        public void ChangeDirectionOfFocusedElement(IInputElement element, FlowDirection direction)
        {
            if (DataContext is TasksViewModel tasksViewModel)
            {
                var newTaskViewModel = tasksViewModel.NewTaskViewModel;

                if (element == NameTextBox)
                    newTaskViewModel.NameDirection = direction;
                else if (element == RelatedToTextBox)
                    newTaskViewModel.RelatedToDirection = direction;
                else if (element == DescriptionRichTextBox)
                    newTaskViewModel.DescriptionDirection = direction;
            }
        }

        public void HighlightSelectedTextInDescription()
        {
            var selection = DescriptionRichTextBox.Selection;
            var range = new TextRange(selection.Start, selection.End);
            range.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Yellow);
        }

        private void LoadDescription(string descriptionXaml)
        {
            try
            {
                DescriptionRichTextBox.Document = (FlowDocument)XamlReader.Parse(descriptionXaml);
            }
            catch (XamlParseException)
            {
                try
                {
                    var withoutImages = descriptionXaml.WithoutBitmapImages();
                    DescriptionRichTextBox.Document = (FlowDocument)XamlReader.Parse(withoutImages);
                }
                catch (XamlParseException)
                {
                    // If we cannot parse descriptionXaml as XAML, treat it as plain text
                    DescriptionRichTextBox.Document.Blocks.Clear();
                    DescriptionRichTextBox.AppendText(descriptionXaml);
                }
            }
        }        

        private void OnKeyboardFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is TasksViewModel tasksViewModel)
            {
                if (sender is System.Windows.Controls.Primitives.DatePickerTextBox || sender is ComboBox)
                {
                    tasksViewModel.IsKeyboardFocusOnTextBox = false;
                }
                else if (sender is TextBox)
                {
                    tasksViewModel.IsKeyboardFocusOnTextBox = true;
                    tasksViewModel.IsKeyboardFocusOnDescription = false;
                }
                else if (sender is RichTextBox)
                {
                    tasksViewModel.IsKeyboardFocusOnTextBox = true;
                    tasksViewModel.IsKeyboardFocusOnDescription = true;
                }
            }
        }                
    }
}
