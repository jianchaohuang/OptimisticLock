using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCodeFirstOptimistic.DataBase
{
    public class Student
    {
        [Key]
        public string StuId { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

    }
}
