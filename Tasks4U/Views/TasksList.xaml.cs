using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Tasks4U.Models;
using Tasks4U.ViewModels;

namespace Tasks4U.Views
{
    /// <summary>
    /// Interaction logic for TasksList.xaml
    /// </summary>
    public partial class TasksList : System.Windows.Controls.UserControl
    {
        private TasksViewModel? _tasksViewModel;
        private ICollectionView? _tasksView;

        public TasksList()
        {
            InitializeComponent();

            DataContextChanged += (s, e) =>
            {
                _tasksViewModel = (TasksViewModel)DataContext;
                var tasks = _tasksViewModel.Tasks;
                var filter = _tasksViewModel.Filter;

                _tasksView = CollectionViewSource.GetDefaultView(tasks);

                if (_tasksView is ListCollectionView listCollectionView)
                    listCollectionView.CustomSort = new CustomSorter();
                
                _tasksView.Filter = task => filter.IsTaskFilteredIn((Task)task);

                filter.IsFilterChanged += () => _tasksView.Refresh();

                _tasksViewModel.IsDateChanged += () => _tasksView.Refresh();
            };           
        }

        public DataGrid TasksGrid => DataGrid;

        public IEnumerable<Task> TasksInView => _tasksView?.Cast<Task>() ?? Enumerable.Empty<Task>();

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

        // This class compares tasks such that finished task are last
        // and other tasks are sorted by final date
        private class CustomSorter: IComparer
        {
            public int Compare(object? x, object? y)
            {
                if (x == null && y == null)
                    return 0;

                if (x == null)
                    return 1;

                if (y == null)
                    return -1;

                var task1 = (Task)x;
                var task2 = (Task)y;
                
                if (task1.Status == TaskStatus.Finished && task2.Status != TaskStatus.Finished)
                    return 1;

                if (task2.Status == TaskStatus.Finished && task1.Status != TaskStatus.Finished)
                    return -1;

                if (task1.NextDateOfTask < task2.NextDateOfTask)
                    return -1;

                if (task1.NextDateOfTask > task2.NextDateOfTask)
                    return 1;

                return 0;
            }
        }       
    }
}
