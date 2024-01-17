using StackExchange.Redis;
using System.Text;

namespace RedisRepository1
{
    public abstract class BaseLock : ILock
    {
        protected const int DefaultRetryCount = 3;

        protected readonly TimeSpan DefaultRetryDelay = new TimeSpan(0, 0, 0, 0, 200);

        protected const double ClockDriveFactor = 0.01;

        protected Dictionary<string, ConnectionMultiplexer> redisMasterDictionary = new Dictionary<string, ConnectionMultiplexer>();

        protected int Quorum => redisMasterDictionary.Count / 2 + 1;

        public BaseLock(params ConnectionMultiplexer[] list)
        {
            foreach (ConnectionMultiplexer connectionMultiplexer in list)
            {
                redisMasterDictionary.Add(connectionMultiplexer.GetEndPoints().First().ToString(), connectionMultiplexer);
            }
        }

        protected static byte[] CreateUniqueLockId()
        {
            return Guid.NewGuid().ToByteArray();
        }

        protected void for_each_redis_registered(Action<ConnectionMultiplexer> action)
        {
            foreach (KeyValuePair<string, ConnectionMultiplexer> item in redisMasterDictionary)
            {
                action(item.Value);
            }
        }

        protected void for_each_redis_registered(Action<string> action)
        {
            foreach (KeyValuePair<string, ConnectionMultiplexer> item in redisMasterDictionary)
            {
                action(item.Key);
            }
        }

        protected async Task for_each_redis_registered(Func<string, Task> action)
        {
            foreach (KeyValuePair<string, ConnectionMultiplexer> item in redisMasterDictionary)
            {
                await action(item.Key);
            }
        }

        protected Lock retry(int retryCount, TimeSpan retryDelay, Func<Lock> action)
        {
            int maxValue = (int)retryDelay.TotalMilliseconds;
            Random random = new Random();
            int num = 0;
            Lock @lock = null;
            while (num++ < retryCount)
            {
                if ((@lock = action()) != null)
                {
                    return @lock;
                }

                Thread.Sleep(random.Next(maxValue));
            }

            return null;
        }

        protected async Task<Lock> retry(int retryCount, TimeSpan retryDelay, Func<Task<Lock>> action)
        {
            int maxRetryDelay = (int)retryDelay.TotalMilliseconds;
            Random rnd = new Random();
            int currentRetry = 0;
            while (currentRetry++ < retryCount)
            {
                Lock result;
                if ((result = await action()) != null)
                {
                    return result;
                }

                Thread.Sleep(rnd.Next(maxRetryDelay));
            }

            return null;
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

        public abstract void Unlock(Lock lockObject);

        public abstract Task UnlockAsync(Lock lockObject);

        public abstract Lock UseLock(Lock existLock);

        public abstract Task<bool> SimpleRecharge(Lock lockObject, TimeSpan ttl);
    }
}
