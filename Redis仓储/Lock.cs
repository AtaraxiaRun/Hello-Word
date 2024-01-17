using StackExchange.Redis;
using System.Runtime.Serialization;

namespace RedisRepository1
{
    [Serializable]
    public class Lock : IDisposable, ISerializable
    {
        private RedisKey resource;

        private RedisKey field;

        private RedisValue val;

        private TimeSpan validity_time;

        private ILock LockInst;

        private bool isDisposed;

        public bool IsAcquired;

        public RedisKey Resource => resource;

        public RedisKey Field => field;

        public RedisValue Value => val;

        public TimeSpan Validity => validity_time;

        public Lock()
        {
        }

        public Lock(RedisKey resource, RedisValue val, TimeSpan validity, ILock lockInst, bool isAcquired = false)
        {
            this.resource = resource;
            this.val = val;
            validity_time = validity;
            LockInst = lockInst;
            IsAcquired = isAcquired;
        }

        public Lock(RedisKey resource, RedisKey field, RedisValue value, TimeSpan validity, ILock lockInst, bool isAcquired = false)
        {
            this.resource = resource;
            this.field = field;
            validity_time = validity;
            LockInst = lockInst;
            IsAcquired = isAcquired;
            val = value;
        }

        public bool Recharge(Lock newLock)
        {
            if (newLock != null && newLock.IsAcquired && newLock.Resource.Equals(resource) && newLock.Field.Equals(field))
            {
                val = newLock.Value;
                validity_time = newLock.Validity;
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (IsAcquired)
                {
                    LockInst.Unlock(this);
                }

                isDisposed = true;
            }
        }

        public ValueTask DisposeAsync()
        {
            return DisposeAsync(disposing: true);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (!isDisposed)
            {
                if (IsAcquired)
                {
                    await LockInst.UnlockAsync(this);
                }

                isDisposed = true;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Resource", Resource.ToString(), typeof(string));
            info.AddValue("Field", Field.ToString(), typeof(string));
            info.AddValue("Value", Value.ToString(), typeof(string));
            info.AddValue("IsAcquired", IsAcquired, typeof(bool));
            info.AddValue("Validity", Validity.Ticks, typeof(long));
        }

        public Lock(SerializationInfo info, StreamingContext context)
        {
            resource = (string)info.GetValue("Resource", typeof(string));
            field = (string)info.GetValue("Field", typeof(string));
            val = (string)info.GetValue("Value", typeof(string));
            IsAcquired = (bool)info.GetValue("IsAcquired", typeof(bool));
            validity_time = TimeSpan.FromTicks((long)info.GetValue("Validity", typeof(long)));
        }
    }
}
