using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCodeFirstOptimistic.DataBase
{
    public class Person
    {
        public int PersonId { get; set; }

        [ConcurrencyCheck]
        public int SocialSecurityNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Money { get; set; }
        //[Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
