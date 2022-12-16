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

namespace Tasks4U.Views
{
    /// <summary>
    /// Interaction logic for TaskDate.xaml
    /// </summary>
    public partial class TaskDate : UserControl
    {
        public TaskDate()
        {
            InitializeComponent();
        }
    }

    public class DatePickerValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            var date = (DateTime)value;

            return date.Date.CompareTo(DateTime.Now) < 0
                ? new ValidationResult(false, "the date can not be before today")
                : new ValidationResult(true, null);
        }
    }
}
