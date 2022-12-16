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
using Tasks4U.ViewModels;

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

            DataContextChanged += (s, e) => AddDataContextEventHandlers();
        }

        private void AddDataContextEventHandlers()
        {
            if (DataContext is TasksViewModel tasksViewModel)
            {
                tasksViewModel.BeforeSave += () => 
                { 
                    // For some reason it works only if we call it twice
                    DesksDataGrid.CommitEdit(); 
                    DesksDataGrid.CommitEdit(); 
                };
            }
        }        
    }
}
