using EFCodeFirstOptimistic.DataBase;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCodeFirstOptimistic
{
    class Program
    {
        static void Main(string[] args)
        {
            var person = new Person
            {
                FirstName = "Cui",
                LastName = "YanWei",
                SocialSecurityNumber = 12345678
            };
            //新增一条记录，保存到数据库中
            using (var con = new EFCodeFirstDbContext())
            {
                con.Persons.Add(person);
                con.SaveChanges();
            }
            var firContext = new EFCodeFirstDbContext();
            //取第一条记录,并修改一个字段：这里是修改了SocialSecurityNumber=123
            //先不保存
            var p1 = firContext.Persons.FirstOrDefault();
            p1.SocialSecurityNumber = 123;
            //再创建一个Context，同样取第一条记录，修改SocialSecurityNumber=456并保存
            using (var secContext = new EFCodeFirstDbContext())
            {
                var p2 = secContext.Persons.FirstOrDefault();

                p2.SocialSecurityNumber = 456;
                secContext.SaveChanges();

            }
            try
            {
                firContext.SaveChanges();
                Console.WriteLine(" 保存成功");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex.Entries.First().Entity.GetType().Name + " 保存失败");
            }
            Console.Read();
        }
    }
}
