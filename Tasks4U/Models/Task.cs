using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public Desk? Desk { get; set; }
        public Frequency TaskFrequency { get; set; }
        public DateOnly IntermediateDate { get; set; }
        public DateOnly FinalDate { get; set; }
        public TaskStatus Status { get; set; }

        // Yes, it's not MVVM to store IsSelected property in the model.
        // But sometimes rules just have to be broken in order to simplify the code...
        [NotMapped]
        public bool IsSelected { get; set; }
    }
}
