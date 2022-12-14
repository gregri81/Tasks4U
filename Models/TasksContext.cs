using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks4U.Models
{
    public class TasksContext: DbContext
    {
        private string _connectionString;

        public TasksContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Task> Tasks => Set<Task>();

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite(_connectionString);
    }
}
