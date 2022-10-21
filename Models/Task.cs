using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks4U.Models
{
    internal enum TaskStatus { NotStarted, InProgress, Pending, Finished};
    internal class Task
    {
        public Task(string name) => Name = name;

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public TaskStatus Status { get; set; }
        public IEnumerable<TaskDate> TaskDates { get; set; } = Enumerable.Empty<TaskDate>();
    }
}
