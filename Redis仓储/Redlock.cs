using StackExchange.Redis;

namespace RedisRepository1
{
    public class Redlock : BaseLock
    {
        private const string UnlockScript = "\r\n            if redis.call(\"get\",KEYS[1]) == ARGV[1] then\r\n                return redis.call(\"del\",KEYS[1])\r\n            else\r\n                return 0\r\n            end";

        private const string RechargeScript = "\r\n            if redis.call(\"get\",KEYS[1]) == ARGV[1] then\r\n                return redis.call(\"set\",KEYS[1],ARGV[2])\r\n            else\r\n                return nil\r\n            end";

        public Redlock(params ConnectionMultiplexer[] list)
            : base(list)
        {
        }

        protected bool LockInstance(string redisServer, string resource, byte[] val, TimeSpan ttl, RedisValue recharge)
        {
            try
            {
                ConnectionMultiplexer connectionMultiplexer = redisMasterDictionary[redisServer];
                return recharge.HasValue ? (!connectionMultiplexer.GetDatabase().ScriptEvaluate("\r\n            if redis.call(\"get\",KEYS[1]) == ARGV[1] then\r\n                return redis.call(\"set\",KEYS[1],ARGV[2])\r\n            else\r\n                return nil\r\n            end", new RedisKey[1] { resource }, new RedisValue[2] { recharge, val }).IsNull) : connectionMultiplexer.GetDatabase().StringSet(resource, val, ttl, When.NotExists);
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected void UnlockInstance(string redisServer, string resource, byte[] val)
        {
            RedisKey[] keys = new RedisKey[1] { resource };
            RedisValue[] values = new RedisValue[1] { val };
            redisMasterDictionary[redisServer].GetDatabase().ScriptEvaluate("\r\n            if redis.call(\"get\",KEYS[1]) == ARGV[1] then\r\n                return redis.call(\"del\",KEYS[1])\r\n            else\r\n                return 0\r\n            end", keys, values);
        }

        protected async Task UnlockInstanceAsync(string redisServer, string resource, byte[] val)
        {
            RedisKey[] keys = new RedisKey[1] { resource };
            RedisValue[] values = new RedisValue[1] { val };
            await redisMasterDictionary[redisServer].GetDatabase().ScriptEvaluateAsync("\r\n            if redis.call(\"get\",KEYS[1]) == ARGV[1] then\r\n                return redis.call(\"del\",KEYS[1])\r\n            else\r\n                return 0\r\n            end", keys, values);
        }

        public Lock Lock(RedisKey resource, TimeSpan ttl)
        {
            return Lock(resource, ttl, RedisValue.Null);
        }

        public Lock Lock(RedisKey resource, TimeSpan ttl, RedisValue recharge)
        {
            RedisValue val = BaseLock.CreateUniqueLockId();
            return retry(3, DefaultRetryDelay, delegate
            {
                try
                {
                    int i = 0;
                    DateTime now = DateTime.Now;
                    for_each_redis_registered(delegate (string redis)
                    {
                        if (LockInstance(redis, resource, val, ttl, recharge))
                        {
                            i++;
                        }
                    });
                    int milliseconds = Convert.ToInt32(ttl.TotalMilliseconds * 0.01 + 2.0);
                    TimeSpan validity = ttl - (DateTime.Now - now) - new TimeSpan(0, 0, 0, 0, milliseconds);
                    if (i >= base.Quorum && validity.TotalMilliseconds > 0.0)
                    {
                        return new Lock(resource, val, validity, this, isAcquired: true);
                    }

                    for_each_redis_registered(delegate (string redis)
                    {
                        UnlockInstance(redis, resource, val);
                    });
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }) ?? new Lock
            {
                IsAcquired = false
            };
        }

        public bool Lock(RedisKey resource, TimeSpan ttl, out Lock lockObject)
        {
            lockObject = Lock(resource, ttl);
            return lockObject != null;
        }

        public override Task<bool> SimpleRecharge(Lock lockObject, TimeSpan ttl)
        {
            return Task.FromResult(lockObject.Recharge(Lock(lockObject.Resource, ttl, true)));
        }

        public override void Unlock(Lock lockObject)
        {
            for_each_redis_registered(delegate (string redis)
            {
                UnlockInstance(redis, lockObject.Resource, lockObject.Value);
            });
        }

        public override async Task UnlockAsync(Lock lockObject)
        {
            List<Task> tasks = new List<Task>();
            for_each_redis_registered(delegate (string redis)
            {
                tasks.Add(UnlockInstanceAsync(redis, lockObject.Resource, lockObject.Value));
            });
            await Task.WhenAll(tasks);
        }

        public override Lock UseLock(Lock existLock)
        {
            if (existLock == null)
            {
                return new Lock
                {
                    IsAcquired = false
                };
            }

            return new Lock(existLock.Resource, existLock.Value, existLock.Validity, this, isAcquired: true);
        }
    }
}
