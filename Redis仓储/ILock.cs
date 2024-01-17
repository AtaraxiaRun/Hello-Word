namespace RedisRepository1
{
    public interface ILock
    {
        void Unlock(Lock lockObject);

        Task UnlockAsync(Lock lockObject);

        Lock UseLock(Lock existLock);

        Task<bool> SimpleRecharge(Lock lockObject, TimeSpan ttl);
    }
}
