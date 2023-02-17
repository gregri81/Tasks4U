using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Tasks4U.Models;
using Tasks4U.ViewModels;

namespace Tasks4U.Views
{
    /// <summary>
    /// Interaction logic for TasksList.xaml
    /// </summary>
    public partial class TasksList : UserControl
    {
        public TasksList()
        {
            InitializeComponent();

            DataContextChanged += (s, e) =>
            {
                var tasksViewModel = (TasksViewModel)DataContext;
                var tasks = tasksViewModel.Tasks;
                var filter = tasksViewModel.Filter;

                var view = CollectionViewSource.GetDefaultView(tasks);
                
                view.SortDescriptions.Add(new SortDescription("FinalDate", ListSortDirection.Descending));

                view.Filter = task => filter.IsTaskFilteredIn((Task)task);

                filter.IsFilterChanged += () => view.Refresh();
            };
        }

        private void TasksDataGridCell_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not DataGridCell cell)
                return;

            // Ignore click on the checkbox column - this is handled seperately
            if (cell.Column.DisplayIndex == 0)
                return;

            var row = DataGridRow.GetRowContainingElement(cell);

            ((TasksViewModel)DataContext).EditTaskCommand.Execute(row.DataContext);
        }
    }
}
