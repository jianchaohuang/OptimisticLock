using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCodeFirstOptimistic.DataBase
{
    public class EFCodeFirstDbContext : DbContext
    {

        public EFCodeFirstDbContext() : base("name=MyStrConn")
        {
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Person> Persons { get; set; }
    }
}
