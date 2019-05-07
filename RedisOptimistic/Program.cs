using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisOptimistic
{
    class Program
    {
        static void Main(string[] args)
        {

            StackExchangeRedisHelper.SetStringValue("key", "100");
            DateTime start = DateTime.Now;
            Parallel.For(1, 1000, index => StackExchangeRedisHelper.Order(index));
            Console.WriteLine("耗时：" + (DateTime.Now - start).Seconds);
            Console.ReadKey();
        }
    }
}
