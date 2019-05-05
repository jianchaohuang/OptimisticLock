using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisOptimistic
{
    /// <summary>
    /// Redis读写帮助类
    /// </summary>
    public class RedisHelper
    {
        private ConnectionMultiplexer redis { get; set; }
        private IDatabase db { get; set; }
        public readonly string conStr = "127.0.0.1:6379";//ConfigurationManager.AppSettings.Get("redisconnection");
        public readonly int dbNum = 1;//Convert.ToInt32(ConfigurationManager.AppSettings.Get("redisdatabase"));
        public RedisHelper()
        {
            var config = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                AllowAdmin = true,
                ConnectTimeout = 15000,
                SyncTimeout = 5000,
                ResponseTimeout = 15000,
                //Password = "Pwd",//Redis数据库密码
                EndPoints = { conStr }// connectionString 为IP:Port 如”192.168.2.110:6379”
            };
            redis = ConnectionMultiplexer.Connect(conStr);
            db = redis.GetDatabase(dbNum);
        }
        public void Order(int num)
        {
            try
            {
                //Thread.Sleep(new Random((int)(Guid.NewGuid().GetHashCode())).Next(1,10)*500);
                //Thread.Sleep(500);
                string clientName = "编号" + num;
                while (true)
                {
                    var val = db.StringGet("key");
                    int intVal = Convert.ToInt32(val);
                    if (intVal > 0)
                    {
                        var transaction = db.CreateTransaction();
                        transaction.AddCondition(Condition.StringEqual("key", val));
                        transaction.StringSetAsync("key", intVal - 1);
                        var isOk = transaction.Execute();
                        if (isOk)
                        {
                            Console.WriteLine("好高兴，顾客:" + clientName + "抢到商品");
                            db.SetAddAsync("clientList", clientName);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("悲剧了，顾客:" + clientName + "没有抢到商品");
                        }
                    }
                    else
                    {
                        Console.WriteLine(clientName + ":库存已经没有了" + DateTime.Now);
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        #region string类型操作
        public bool KeyExists(string key)
        {
            return db.KeyExists(key);
        }
        /// <summary>
        /// set or update the value for string key 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetStringValue(string key, string value)
        {
            return db.StringSet(key, value);
        }
        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public bool SetStringKey(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            return db.StringSet(key, value, expiry);
        }
        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool SetStringKey<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?))
        {

            string json = JsonConvert.SerializeObject(obj);
            return db.StringSet(key, json, expiry);
        }
        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetStringKey<T>(string key) where T : class
        {
            var result = db.StringGet(key);
            if (string.IsNullOrEmpty(result))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<T>(result);
        }
        /// <summary>
        /// get the value for string key 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetStringValue(string key)
        {
            return db.StringGet(key);
        }

        /// <summary>
        /// Delete the value for string key 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool DeleteStringKey(string key)
        {
            return db.KeyDelete(key);
        }

        /// <summary>
        /// the value of key after the increment 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long StringIncrement(string key, long value)
        {
            return db.StringIncrement(key, value);
        }
        #endregion

        #region 哈希类型操作
        /// <summary>
        /// set or update the HashValue for string key 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashkey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetHashValue(string key, string hashkey, string value)
        {
            return db.HashSet(key, hashkey, value);
        }
        /// <summary>
        /// set or update the HashValue for string key 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="hashkey"></param>
        /// <param name="t">defined class</param>
        /// <returns></returns>
        public bool SetHashValue<T>(String key, string hashkey, T t) where T : class
        {
            var json = JsonConvert.SerializeObject(t);
            return db.HashSet(key, hashkey, json);
        }
        /// <summary>
        /// 保存一个集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Redis Key</param>
        /// <param name="list">数据集合</param>
        /// <param name="getModelId"></param>
        public void HashSet<T>(string key, List<T> list, Func<T, string> getModelId)
        {
            List<HashEntry> listHashEntry = new List<HashEntry>();
            foreach (var item in list)
            {
                string json = JsonConvert.SerializeObject(item);
                listHashEntry.Add(new HashEntry(getModelId(item), json));
            }
            db.HashSet(key, listHashEntry.ToArray());
        }

        /// <summary>
        /// get the HashValue for string key  and hashkey
        /// </summary>
        /// <param name="key">Represents a key that can be stored in redis</param>
        /// <param name="hashkey"></param>
        /// <returns></returns>
        public RedisValue GetHashValue(string key, string hashkey)
        {
            RedisValue result = db.HashGet(key, hashkey);
            return result;
        }
        /// <summary>
        /// get the HashValue for string key  and hashkey
        /// </summary>
        /// <param name="key">Represents a key that can be stored in redis</param>
        /// <param name="hashkey"></param>
        /// <returns></returns>
        public T GetHashValue<T>(string key, string hashkey) where T : class
        {
            string result = db.HashGet(key, hashkey);
            if (string.IsNullOrEmpty(result))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<T>(result);
        }
        /// <summary>
        /// delete the HashValue for string key  and hashkey
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashkey"></param>
        /// <returns></returns>
        public bool DeleteHashValue(string key, string hashkey)
        {
            return db.HashDelete(key, hashkey);
        }
        #endregion

        #region Lists

        public long AddList<T>(string listId, T t)
        {
            var value = JsonConvert.SerializeObject(t);
            return db.ListLeftPush(listId, value);
        }

        public List<T> GetList<T>(string listId, long start = 0, long stop = -1)
        {
            var result = new List<T>();
            var list = db.ListRange(listId, start, stop).ToList();
            if (list.Count > 0)
            {
                list.ForEach(x =>
                {
                    var value = JsonConvert.DeserializeObject<T>(x);
                    result.Add(value);
                });
            }
            return result;
        }
        #endregion

        #region Extend
        //使用Keys *模糊匹配Key
        public List<string> GetKeys(string key)
        {
            var result = (string[])db.ScriptEvaluate(LuaScript.Prepare("return redis.call('KEYS',@keypattern)"), new { keypattern = key });
            return result.ToList();
        }

        #endregion

        #region Sets

        #region 同步
        public bool SetAdd(string key, string value)
        {
            return db.SetAdd(key, value);
        }

        public bool SetRemove(string key, string value)
        {
            return db.SetRemove(key, value);
        }

        public List<T> SetMembers<T>(string key)
        {
            var result = new List<T>();
            var list = db.SetMembers(key).ToList();
            if (list.Count > 0)
            {
                list.ForEach(x =>
                {
                    var value = JsonConvert.DeserializeObject<T>(x);
                    result.Add(value);
                });
            }
            return result;
        }
        public List<string> SetMembers(string key)
        {
            var result = new List<string>();
            var list = db.SetMembers(key).ToList();
            if (list.Count > 0)
            {
                list.ForEach(x =>
                {
                    result.Add(x.ToString());
                });
            }
            return result;
        }
        public List<T> SetScan<T>(string key, int cursor, string pattern, int count)
        {
            List<T> list = new List<T>();
            var result = db.ScriptEvaluate(LuaScript.Prepare("return redis.call('SSCAN',@key,@cursor,'MATCH',@pattern,'COUNT',@count)"), new { key = key, cursor = cursor, pattern = "*" + pattern + "*", count = count });
            if (!result.IsNull)
            {
                var vals = ((StackExchange.Redis.RedisResult[])((StackExchange.Redis.RedisResult[])result)[1]);
                foreach (var item in vals)
                {
                    list.Add(JsonConvert.DeserializeObject<T>(item.ToString()));
                }
            }
            return list;
        }
        public List<string> SetScan(string key, int cursor, string pattern, int count)
        {
            List<string> list = new List<string>();
            var result = db.ScriptEvaluate(LuaScript.Prepare("return redis.call('SSCAN',@key,@cursor,'MATCH',@pattern,'COUNT',@count)"), new { key = key, cursor = cursor, pattern = pattern, count = count });
            if (!result.IsNull)
            {
                var vals = ((StackExchange.Redis.RedisResult[])((StackExchange.Redis.RedisResult[])result)[1]);
                foreach (var item in vals)
                {
                    list.Add(item.ToString());
                }
            }
            return list;
        }
        public List<T> SetScan<T>(string key, int cursor, string pattern, int count, out int nextCursor)
        {
            List<T> list = new List<T>();
            var result = db.ScriptEvaluate(LuaScript.Prepare("return redis.call('SSCAN',@key,@cursor,'MATCH',@pattern,'COUNT',@count)"), new { key = key, cursor = cursor, pattern = "*" + pattern + "*", count = count });
            nextCursor = 0;
            if (!result.IsNull)
            {
                nextCursor = (int)((StackExchange.Redis.RedisResult[])result)[0];
                var vals = ((StackExchange.Redis.RedisResult[])((StackExchange.Redis.RedisResult[])result)[1]);
                foreach (var item in vals)
                {
                    list.Add(JsonConvert.DeserializeObject<T>(item.ToString()));
                }
            }
            return list;
        }

        public long SetLength(string key)
        {
            return db.SetLength(key);
        }

        #endregion

        #endregion
    }
}
