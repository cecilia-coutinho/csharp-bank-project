using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankProject
{
    public class AppDataContext : DbContext //stablish conexion with DB
    {
        // override method - config object to connect the DB
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            //take conexion string to DB
            optionsBuilder.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = App; Integrated Security = True;");
        }

        //map tables to DB. Determine the "user" type to be stored
        //add migration command in PM Console
        public DbSet<User> Users { get; set; }
    }
}
