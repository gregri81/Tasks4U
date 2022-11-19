using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks4U.Models
{
    public enum TaskStatus { NotStarted, InProgress, Pending, Finished};

    public enum Frequency { Once, EveryWeek, EveryMonth, EveryYear };

    public class Task
    {
        public Task(string name) => Name = name;

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public Frequency TaskFrequency { get; set; }
        public DateOnly IntermmediateDate { get; set; }
        public DateOnly FinalDate { get; set; }
        public TaskStatus Status { get; set; }
    }
}
