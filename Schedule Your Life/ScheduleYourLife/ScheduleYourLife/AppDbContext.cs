using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ScheduleYourLife
{
    public class AppDbContext : DbContext
    {
        public DbSet<TaskItems> TaskItems { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=schedule.db");
        }
    }
}
