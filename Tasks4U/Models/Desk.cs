using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks4U.Models
{
    public class Desk
    {
        // We need an empty constructor, because otherwise the datagrid does not enable the user to add rows
        public Desk() { }

        public Desk(string name) => Name = name;

        public int ID { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
