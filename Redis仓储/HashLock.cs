using StackExchange.Redis;
using System.Text;

namespace RedisRepository1
{
    public class HashLock : BaseLock
    {
        private const string LockScript = " return (( tonumber(ARGV[1])-(tonumber(redis.call('hget',KEYS[1],KEYS[2])) or 0)) )>0  and redis.call('hset',KEYS[1],KEYS[2],ARGV[2]) ";

        private const string UnlockScript = "\r\n            if redis.call(\"hget\",KEYS[1],KEYS[2]) == ARGV[1] then\r\n                return redis.call(\"hdel\",KEYS[1],KEYS[2])\r\n            else\r\n                return 0\r\n            end";

        private const string RechargeScript = "return (( tonumber(ARGV[1])-(tonumber(redis.call('hget',KEYS[1],KEYS[2])) or ARGV[1])) )==0  and redis.call('hset',KEYS[1],KEYS[2],ARGV[2]) ";

        public HashLock(params ConnectionMultiplexer[] list)
            : base(list)
        {
        }

        protected async Task<bool> LockInstance(string redisServer, string key, string field, TimeSpan ttl, DateTime baseTime, RedisValue recharge)
        {
            bool result;
            try
            {
                result = !(await redisMasterDictionary[redisServer].GetDatabase().ScriptEvaluateAsync(recharge.HasValue ? "return (( tonumber(ARGV[1])-(tonumber(redis.call('hget',KEYS[1],KEYS[2])) or ARGV[1])) )==0  and redis.call('hset',KEYS[1],KEYS[2],ARGV[2]) " : " return (( tonumber(ARGV[1])-(tonumber(redis.call('hget',KEYS[1],KEYS[2])) or 0)) )>0  and redis.call('hset',KEYS[1],KEYS[2],ARGV[2]) ", new RedisKey[2] { key, field }, new RedisValue[2]
                {
                recharge.HasValue ? recharge : ((RedisValue)baseTime.Ticks),
                baseTime.Add(ttl).Ticks
                })).IsNull;
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        protected void UnlockInstance(string redisServer, string key, string field)
        {
            redisMasterDictionary[redisServer].GetDatabase().HashDelete(key, field);
        }

        protected async Task UnlockInstanceAsync(string redisServer, string key, string field, RedisValue val)
        {
            await redisMasterDictionary[redisServer].GetDatabase().ScriptEvaluateAsync("\r\n            if redis.call(\"hget\",KEYS[1],KEYS[2]) == ARGV[1] then\r\n                return redis.call(\"hdel\",KEYS[1],KEYS[2])\r\n            else\r\n                return 0\r\n            end", new RedisKey[2] { key, field }, new RedisValue[1] { val });
        }

        public async Task<Lock> Lock(RedisKey key, RedisKey field, TimeSpan ttl)
        {
            return await Lock(key, field, ttl, RedisValue.Null);
        }

        public async Task<Lock> Lock(RedisKey key, RedisKey field, TimeSpan ttl, RedisValue recharge)
        {
            DateTime baseTime = DateTime.Now;
            RedisValue val = baseTime.Add(ttl).Ticks;
            return (await retry(3, DefaultRetryDelay, async delegate
            {
                _ = 1;
                try
                {
                    int i = 0;
                    DateTime startTime = DateTime.Now;
                    await for_each_redis_registered(async delegate (string redis)
                    {
                        if (await LockInstance(redis, key, field, ttl, baseTime, recharge))
                        {
                            i++;
                        }
                    });
                    int milliseconds = Convert.ToInt32(ttl.TotalMilliseconds * 0.01 + 2.0);
                    TimeSpan validity = ttl - (DateTime.Now - startTime) - new TimeSpan(0, 0, 0, 0, milliseconds);
                    if (i >= base.Quorum && validity.TotalMilliseconds > 0.0)
                    {
                        return new Lock(key, field, val, validity, this, isAcquired: true);
                    }

                    await for_each_redis_registered(async delegate (string redis)
                    {
                        await UnlockInstanceAsync(redis, key, field, val);
                    });
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            })) ?? new Lock
            {
                IsAcquired = false
            };
        }

        public override void Unlock(Lock lockObject)
        {
            for_each_redis_registered(delegate (string redis)
            {
                UnlockInstance(redis, lockObject.Resource, lockObject.Field);
            });
        }

        public override async Task<bool> SimpleRecharge(Lock lockObject, TimeSpan ttl)
        {
            return lockObject.Recharge(await Lock(lockObject.Resource, lockObject.Field, ttl, lockObject.Value));
        }

        public override async Task UnlockAsync(Lock lockObject)
        {
            List<Task> taskAll = new List<Task>();
            for_each_redis_registered(delegate (string redis)
            {
                taskAll.Add(UnlockInstanceAsync(redis, lockObject.Resource, lockObject.Field, lockObject.Value));
            });
            await Task.WhenAll(taskAll.ToArray());
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(GetType().FullName);
            stringBuilder.AppendLine("Registered Connections:");
            foreach (KeyValuePair<string, ConnectionMultiplexer> item in redisMasterDictionary)
            {
                stringBuilder.AppendLine(item.Value.GetEndPoints().First().ToString());
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="existLock"></param>
        /// <returns></returns>
        public override Lock UseLock(Lock existLock)
        {
            if (existLock == null)
            {
                return new Lock
                {
                    IsAcquired = false
                };
            }

            return new Lock(existLock.Resource, existLock.Field, existLock.Value, existLock.Validity, this, isAcquired: true);
        }
    }
}
