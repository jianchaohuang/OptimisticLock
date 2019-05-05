using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EFOptimistic
{
    public class PersonDAL
    {
        

        public Person GetPerson(int id)
        {
            using (EntitiesContext context = new EntitiesContext())
            {
                IQueryable<Person> list = context.Person.Where(x => x.Id == id);
                return list.First();
            }
        }

        //更新对象
        public void Update(Person person)
        {
            using (EntitiesContext context = new EntitiesContext())
            {
                var obj = context.Person.Where(x => x.Id == person.Id).First();
                try
                {
                    if (obj != null)
                        context.ApplyCurrentValues("Person", person);
                    //虚拟操作，保证数据被同步加载
                    Thread.Sleep(100);
                    context.SaveChanges();
                    //显示第一次更新后的数据属性
                    this.DisplayProperty("Current", person);
                }
                catch (System.Data.OptimisticConcurrencyException ex)
                {
                    //显示发生OptimisticConcurrencyException异常所输入的数据属性
                    this.DisplayProperty("OptimisticConcurrencyException", person);

                    if (person.EntityKey == null)
                        person.EntityKey = new System.Data.EntityKey("EntitiesContext.Person",
                                   "Id", person.Id);
                    //保持上下文当中对象的现有属性
                    context.Refresh(RefreshMode.StoreWins, person);
                    context.SaveChanges();
                }
            }
        }

        //显示对象相关属性
        public void DisplayProperty(string message, Person person)
        {
            String data = string.Format("{0}\n  Person Message:\n    Id:{1}  FirstName:{2}  " +
                "SecondName:{3}  Age:{4}\n    Address:{5}  Telephone:{6}  EMail:{7}\n",
                message, person.Id, person.FirstName, person.SecondName, person.Age,
                person.Address, person.Tel, person.Email);
            Console.WriteLine(data);
        }
    }
}
