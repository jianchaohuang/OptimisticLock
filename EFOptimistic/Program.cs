using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EFOptimistic
{
    class Program
    {
        delegate void MyDelegate(Person person);

        public static void Main(string[] args)
        {
            //在更新数据前显示对象信息
            PersonDAL personDAL = new PersonDAL();
            var beforeObj = personDAL.GetPerson(1);
            personDAL.DisplayProperty("Before", beforeObj);

            //更新Person的FirstName、SecondName属性
            Person person1 = new Person();
            person1.Id = 1;
            person1.FirstName = "Mike";
            person1.SecondName = "Wang";
            person1.Age = 32;
            person1.Address = "Tianhe";
            person1.Tel = "13660123456";
            person1.Email = "Leslie@163.com";

            //更新Person的FirstName、SecondName属性
            Person person2 = new Person();
            person2.Id = 1;
            person2.FirstName = "Rose";
            person2.SecondName = "Chen";
            person2.Age = 32;
            person2.Address = "Tianhe";
            person2.Tel = "13660123456";
            person2.Email = "Leslie@163.com";

            //使用异步方式更新数据
            MyDelegate myDelegate = new MyDelegate(personDAL.Update);
            myDelegate.BeginInvoke(person1, null, null);
            myDelegate.BeginInvoke(person2, null, null);
            //显示完成更新后数据源中的对应属性
            Thread.Sleep(1000);
            var afterObj = personDAL.GetPerson(1);
            personDAL.DisplayProperty("After", afterObj);
            Console.ReadKey();
        }
    }
}
