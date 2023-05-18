using DriverSNT2_L.DataControl;
using DriverSNT2_L.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DriverSNT2_L.Context
{
    internal class DataContext : DbContext
    {
        private readonly string _connectionString = @ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString;


        public DbSet<ProjectObject> ProjectObjects { get; set; }
        public DbSet<Counter> Counters { get; set; }
        public DbSet<ListValue> ListValues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<History>().Property(his => his.DateTime).HasColumnType("datetime");
            modelBuilder.Entity<ListValue>().Property(lv => lv.DateTimeUpdate).HasColumnType("datetime");
            modelBuilder.Entity<ListValue>().Property(lv => lv.DateTimeUpdate).HasDefaultValue("1753-01-01 00:00:00.000");
            modelBuilder.Entity<ListValue>().Property(lv => lv.Description).HasDefaultValue("");           
            modelBuilder.Entity<ListValue>().Property(lv => lv.Csd_Changed).HasDefaultValue(false);           
            modelBuilder.Entity<ListValue>().Property(lv => lv.Has_History).HasDefaultValue(false);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);            
        }        
    }
}
