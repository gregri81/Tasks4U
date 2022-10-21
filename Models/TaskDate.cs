using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks4U.Models
{
    internal enum Frequency { Once, EveryWeek, EveryMonth, EveryYear };
    internal class TaskDate
    {
        public int ID { get; set; }
        public DateOnly Date { get; set; }
        public Frequency Frequency { get; set; }
        public bool IsFinal { get; set; } // final vs inermediate
    }
}
