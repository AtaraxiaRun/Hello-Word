using StackExchange.Redis;

namespace RedisRepository1
{
    public interface IRedisManager
    {
        bool Set<TEntity>(string key, TEntity entity, TimeSpan? cacheTime = null, int? idx = null);

        bool Set<TEntity>(string key, TEntity[] entities, TimeSpan? cacheTime = null, int? idx = null);

        bool Set<TEntity>(string key, List<TEntity> entities, TimeSpan? cacheTime = null, int? idx = null);

        Task<bool> SetAsync<TEntity>(string key, TEntity entity, TimeSpan? cacheTime = null, int? idx = null);

        Task<bool> SetAsync<TEntity>(string key, TEntity[] entities, TimeSpan? cacheTime = null, int? idx = null);

        Task<bool> SetAsync<TEntity>(string key, List<TEntity> entities, TimeSpan? cacheTime = null, int? idx = null);

        Task<bool> HashSetAsync(string key, string hashField, string value, int? idx = null);

        long Count(string key, int? idx = null);

        Task<long> CountAsync(string key, int? idx = null);

        Task<RedisValueWithExpiry> StringExpireAsync(string key, int? idx = null);

        bool Exists(string key, int? idx = null);

        Task<bool> ExistsAsync(string key, int? idx = null);

        bool Expire(string key, TimeSpan cacheTime, int? idx = null);

        Task<bool> ExpireAsync(string key, TimeSpan cacheTime, int? idx = null);

        bool Remove(string key, int? idx = null);

        bool Remove(string[] keys, int? idx = null);

        bool Remove(List<string> keys, int? idx = null);

        Task<bool> RemoveAsync(string key, int? idx = null);

        Task<bool> RemoveAsync(string[] keys, int? idx = null);

        Task<bool> RemoveAsync(List<string> keys, int? idx = null);

        Task<bool> HashRemoveAsync(string key, string hashField, int? idx = null);

        string BlockingDequeue(string key, int? idx = null);

        Task<string> BlockingDequeueAsync(string key, int? idx = null);

        void Enqueue<TEntity>(string key, TEntity entity, int? idx = null);

        Task EnqueueAsync<TEntity>(string key, TEntity entity, int? idx = null);

        long Increment(string key, int? idx = null);

        Task<long> IncrementAsync(string key, int? idx = null);

        Task<long> IncrementAsync(string key, string value, int? idx = null);

        long Decrement(string key, string value, int? idx = null);

        Task<long> DecrementAsync(string key, int? idx = null);

        Task<long> DecrementAsync(string key, string value, int? idx = null);

        Task<Dictionary<string, string>> HashGetAllAsync(string key, int? idx = null);

        Task<bool[]> HashSetAsync(string key, Dictionary<string, string> kves, When when = When.Always, int? idx = null);

        Task<RedisValue> HashGetAsync(string key, string hashField, int? idx = null);

        Task<RedisValue[]> HashGetAsync(string key, string[] hashFields, int? idx = null);

        Task<bool> HashExistAsync(string key, string hashField, int? idx = null);

        TEntity Get<TEntity>(string key, int? idx = null);

        List<TEntity> GetList<TEntity>(string key, int? idx = null);

        TEntity[] GetArray<TEntity>(string key, int? idx = null);

        Task<TEntity> GetAsync<TEntity>(string key, int? idx = null);

        Task<List<TEntity>> GetListAsync<TEntity>(string key, int? idx = null);

        Task<TEntity[]> GetArrayAsync<TEntity>(string key, int? idx = null);

        bool Setnx(string key, string value, TimeSpan? expire = null, int? idx = null);

        Task<bool> SetnxAsync(string key, string value, TimeSpan? expire = null, int? idx = null);

        bool HashSetnx(string key, string field, string value, int? idx = null);

        Task<bool> HashSetnxAsync(string key, string field, string value, int? idx = null);

        HashLock GetHashLock();

        Redlock GetRedlock();
    }
}
