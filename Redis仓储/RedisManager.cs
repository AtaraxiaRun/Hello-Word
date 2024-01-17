using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace RedisRepository1
{
    public class RedisManager : IRedisManager
    {
        private readonly IOptions<RedisOptions> _redisOptions;

        private ConnectionMultiplexer _conn;

        private static string _connStr;

        private Redlock _redlock;

        private HashLock _hashlock;

        public RedisManager(IOptions<RedisOptions> redisOptions)
        {
            _redisOptions = redisOptions;
            _connStr = $"{_redisOptions.Value.HostName}:{_redisOptions.Value.Port},allowAdmin=true,password={_redisOptions.Value.Password},defaultdatabase={_redisOptions.Value.Defaultdatabase}";
            RedisConnection();
        }

        private void RedisConnection()
        {
            try
            {
                Console.WriteLine("Redis config: " + _connStr);
                _conn = ConnectionMultiplexer.Connect(_connStr);
                Console.WriteLine("Redis manager started!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Redis connection error: " + ex.Message);
            }
        }

        private IDatabase GetDatabase(int? idx)
        {
            try
            {
                return _conn.GetDatabase(idx ?? _redisOptions.Value.Defaultdatabase);
            }
            catch
            {
                _conn = ConnectionMultiplexer.Connect(_connStr);
                Console.WriteLine("Redis manager reconnection!");
                return _conn.GetDatabase(idx ?? _redisOptions.Value.Defaultdatabase);
            }
        }

        public bool Set<TEntity>(string key, TEntity entity, TimeSpan? cacheTime = null, int? idx = null)
        {
            if (Exists(key, idx))
            {
                Remove(key, idx);
            }

            bool flag = GetDatabase(idx).StringSet(key, JsonConvert.SerializeObject(entity));
            if (cacheTime.HasValue)
            {
                if (flag)
                {
                    return Expire(key, cacheTime.Value, idx);
                }

                return false;
            }

            return flag;
        }

        public bool Set<TEntity>(string key, TEntity[] entities, TimeSpan? cacheTime = null, int? idx = null)
        {
            if (Exists(key, idx))
            {
                Remove(key, idx);
            }

            RedisValue[] array = ((IEnumerable<TEntity>)entities).Select((Func<TEntity, RedisValue>)((TEntity p) => JsonConvert.SerializeObject(p))).ToArray();
            bool flag = GetDatabase(idx).SetAdd(key, array) == array.Length;
            if (cacheTime.HasValue)
            {
                if (flag)
                {
                    return Expire(key, cacheTime.Value, idx);
                }

                return false;
            }

            return flag;
        }

        public bool Set<TEntity>(string key, List<TEntity> entities, TimeSpan? cacheTime = null, int? idx = null)
        {
            if (Exists(key, idx))
            {
                Remove(key, idx);
            }

            return Set(key, entities.ToArray(), cacheTime, idx);
        }

        public async Task<bool> SetAsync<TEntity>(string key, TEntity entity, TimeSpan? cacheTime = null, int? idx = null)
        {
            if (await ExistsAsync(key, idx))
            {
                await RemoveAsync(key, idx);
            }

            return await GetDatabase(idx).StringSetAsync(key, JsonConvert.SerializeObject(entity), cacheTime);
        }

        public async Task<bool> SetAsync<TEntity>(string key, TEntity[] entities, TimeSpan? cacheTime = null, int? idx = null)
        {
            if (await ExistsAsync(key, idx))
            {
                await RemoveAsync(key, idx);
            }

            RedisValue[] redisValues = ((IEnumerable<TEntity>)entities).Select((Func<TEntity, RedisValue>)((TEntity p) => JsonConvert.SerializeObject(p))).ToArray();
            bool flag = await GetDatabase(idx).SetAddAsync(key, redisValues) == redisValues.Length;
            if (cacheTime.HasValue)
            {
                bool flag2 = flag;
                if (flag2)
                {
                    flag2 = await ExpireAsync(key, cacheTime.Value, idx);
                }

                return flag2;
            }

            return flag;
        }

        public async Task<bool> SetAsync<TEntity>(string key, List<TEntity> entities, TimeSpan? cacheTime = null, int? idx = null)
        {
            if (await ExistsAsync(key, idx))
            {
                await RemoveAsync(key, idx);
            }

            return await SetAsync(key, entities.ToArray(), cacheTime, idx);
        }

        public async Task<bool> HashSetAsync(string key, string hashField, string value, int? idx = null)
        {
            return await GetDatabase(idx).HashSetAsync(key, hashField, value);
        }

        public async Task<bool[]> HashSetAsync(string key, Dictionary<string, string> kves, When when = When.Always, int? idx = null)
        {
            IBatch batch = GetDatabase(idx).CreateBatch();
            Task<bool>[] tasks = kves.Select((KeyValuePair<string, string> p) => batch.HashSetAsync(key, p.Key, p.Value, when)).ToArray();
            batch.Execute();
            return await Task.WhenAll(tasks);
        }

        public long Count(string key, int? idx = null)
        {
            return GetDatabase(idx).ListLength(key);
        }

        public async Task<long> CountAsync(string key, int? idx = null)
        {
            return await GetDatabase(idx).ListLengthAsync(key);
        }

        public async Task<RedisValueWithExpiry> StringExpireAsync(string key, int? idx = null)
        {
            return await GetDatabase(idx).StringGetWithExpiryAsync(key);
        }

        public bool Exists(string key, int? idx = null)
        {
            return GetDatabase(idx).KeyExists(key);
        }

        public async Task<bool> ExistsAsync(string key, int? idx = null)
        {
            return await GetDatabase(idx).KeyExistsAsync(key);
        }

        public bool Expire(string key, TimeSpan cacheTime, int? idx = null)
        {
            return GetDatabase(idx).KeyExpire(key, DateTime.Now.AddSeconds(int.Parse(cacheTime.TotalSeconds.ToString())));
        }

        public async Task<bool> ExpireAsync(string key, TimeSpan cacheTime, int? idx = null)
        {
            return await GetDatabase(idx).KeyExpireAsync(key, DateTime.Now.AddSeconds(int.Parse(cacheTime.TotalSeconds.ToString())));
        }

        public bool Remove(string key, int? idx = null)
        {
            return GetDatabase(idx).KeyDelete(key);
        }

        public bool Remove(string[] keys, int? idx = null)
        {
            RedisKey[] array = ((IEnumerable<string>)keys).Select((Func<string, RedisKey>)((string p) => JsonConvert.SerializeObject(p))).ToArray();
            return GetDatabase(idx).KeyDelete(array) == array.Length;
        }

        public bool Remove(List<string> keys, int? idx = null)
        {
            return Remove(keys.ToArray(), idx);
        }

        public async Task<bool> RemoveAsync(string key, int? idx = null)
        {
            return await GetDatabase(idx).KeyDeleteAsync(key);
        }

        public async Task<bool> RemoveAsync(string[] keys, int? idx = null)
        {
            RedisKey[] redisKeys = ((IEnumerable<string>)keys).Select((Func<string, RedisKey>)((string p) => JsonConvert.SerializeObject(p))).ToArray();
            return await GetDatabase(idx).KeyDeleteAsync(redisKeys) == redisKeys.Length;
        }

        public async Task<bool> RemoveAsync(List<string> keys, int? idx = null)
        {
            return await RemoveAsync(keys.ToArray(), idx);
        }

        public async Task<bool> HashRemoveAsync(string key, string hashField, int? idx = null)
        {
            return await GetDatabase(idx).HashDeleteAsync(key, hashField);
        }

        public string BlockingDequeue(string key, int? idx = null)
        {
            return GetDatabase(idx).ListRightPop(key);
        }

        public async Task<string> BlockingDequeueAsync(string key, int? idx = null)
        {
            return await GetDatabase(idx).ListRightPopAsync(key);
        }

        public void Enqueue<TEntity>(string key, TEntity entity, int? idx = null)
        {
            GetDatabase(idx).ListLeftPush(key, JsonConvert.SerializeObject(entity));
        }

        public async Task EnqueueAsync<TEntity>(string key, TEntity entity, int? idx = null)
        {
            await GetDatabase(idx).ListLeftPushAsync(key, JsonConvert.SerializeObject(entity));
        }

        public long Increment(string key, int? idx = null)
        {
            return GetDatabase(idx).StringIncrement(key, 1L);
        }

        public async Task<long> IncrementAsync(string key, int? idx = null)
        {
            return await GetDatabase(idx).StringIncrementAsync(key, 1L);
        }

        public async Task<long> IncrementAsync(string key, string value, int? idx = null)
        {
            return await GetDatabase(idx).HashIncrementAsync(key, value, 1L);
        }

        public long Decrement(string key, string value, int? idx = null)
        {
            return GetDatabase(idx).HashDecrement(key, value, 1L);
        }

        public async Task<long> DecrementAsync(string key, int? idx = null)
        {
            return await GetDatabase(idx).StringDecrementAsync(key, 1L);
        }

        public async Task<long> DecrementAsync(string key, string value, int? idx = null)
        {
            return await GetDatabase(idx).HashDecrementAsync(key, value, 1L);
        }

        public async Task<Dictionary<string, string>> HashGetAllAsync(string key, int? idx = null)
        {
            if (!(await ExistsAsync(key, idx)))
            {
                return null;
            }

            HashEntry[] array = await GetDatabase(idx).HashGetAllAsync(key);
            if (array.Length != 0)
            {
                return array.ToStringDictionary();
            }

            return null;
        }

        public async Task<RedisValue> HashGetAsync(string key, string hashField, int? idx = null)
        {
            if (!(await ExistsAsync(key, idx)))
            {
                return default(RedisValue);
            }

            return await GetDatabase(idx).HashGetAsync(key, hashField);
        }

        public async Task<RedisValue[]> HashGetAsync(string key, string[] hashFields, int? idx = null)
        {
            if (!(await ExistsAsync(key, idx)))
            {
                return null;
            }

            return await GetDatabase(idx).HashGetAsync(key, hashFields.ToRedisValueArray());
        }

        public async Task<bool> HashExistAsync(string key, string hashField, int? idx = null)
        {
            if (!(await ExistsAsync(key, idx)))
            {
                return false;
            }

            return await GetDatabase(idx).HashExistsAsync(key, hashField);
        }

        public TEntity Get<TEntity>(string key, int? idx = null)
        {
            if (!Exists(key, idx))
            {
                return default(TEntity);
            }
            var entity = GetDatabase(idx).StringGet(key);
            return JsonConvert.DeserializeObject<TEntity>(entity);
        }

        public List<TEntity> GetList<TEntity>(string key, int? idx = null)
        {
            if (!Exists(key, idx))
            {
                return null;
            }

            return (from p in GetDatabase(idx).SetMembers(key)
                    select JsonConvert.DeserializeObject<TEntity>(p)).ToList();
        }

        public TEntity[] GetArray<TEntity>(string key, int? idx = null)
        {
            if (!Exists(key, idx))
            {
                return null;
            }

            return (from p in GetDatabase(idx).SetMembers(key)
                    select JsonConvert.DeserializeObject<TEntity>(p)).ToArray();
        }

        public async Task<TEntity> GetAsync<TEntity>(string key, int? idx = null)
        {
            if (!(await ExistsAsync(key, idx)))
            {
                return default(TEntity);
            }

            return JsonConvert.DeserializeObject<TEntity>(await GetDatabase(idx).StringGetAsync(key));
        }

        public async Task<List<TEntity>> GetListAsync<TEntity>(string key, int? idx = null)
        {
            if (!(await ExistsAsync(key, idx)))
            {
                return null;
            }

            return (await GetDatabase(idx).SetMembersAsync(key)).Select((RedisValue p) => JsonConvert.DeserializeObject<TEntity>(p)).ToList();
        }

        public async Task<TEntity[]> GetArrayAsync<TEntity>(string key, int? idx = null)
        {
            if (!(await ExistsAsync(key, idx)))
            {
                return null;
            }

            return (await GetDatabase(idx).SetMembersAsync(key)).Select((RedisValue p) => JsonConvert.DeserializeObject<TEntity>(p)).ToArray();
        }

        public bool Setnx(string key, string value, TimeSpan? expire = null, int? idx = null)
        {
            return GetDatabase(idx).StringSet(key, value, expire, When.NotExists);
        }

        public async Task<bool> SetnxAsync(string key, string value, TimeSpan? expire = null, int? idx = null)
        {
            return await GetDatabase(idx).StringSetAsync(key, value, expire, When.NotExists);
        }

        public bool HashSetnx(string key, string field, string value, int? idx = null)
        {
            return GetDatabase(idx).HashSet(key, field, value, When.NotExists);
        }

        public async Task<bool> HashSetnxAsync(string key, string field, string value, int? idx = null)
        {
            return await GetDatabase(idx).HashSetAsync(key, field, value, When.NotExists);
        }

        /// <summary>
        /// 获取锁对象
        /// </summary>
        /// <returns></returns>
        public HashLock GetHashLock()
        {
            return _hashlock ?? (_hashlock = new HashLock(_conn));
        }

        public Redlock GetRedlock()
        {
            return _redlock ?? (_redlock = new Redlock(_conn));
        }
    }
}
