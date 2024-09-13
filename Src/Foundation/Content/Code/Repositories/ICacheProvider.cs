using System;

namespace DEWAXP.Foundation.Content.Repositories
{
    /// <summary>
    /// A simple cache provider interface allowing for storage and retrieval of data
    /// </summary>
    public interface ICacheProvider : IDisposable
    {
        /// <summary>
        /// Stores a cache item with the given key. If another object is already stored against the specified key, it will be overwritten.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key to be used</param>
        /// <param name="item">The cache item to be stored</param>
        void Store<T>(string key, CacheItem<T> item);

        /// <summary>
        /// Removes the cache entry with the given key if the key exists.
        /// </summary>
        /// <param name="key">The key of the cache entry to be removed</param>
        void Remove(string key);

        /// <summary>
        /// Attempts to retrieve a stored cache entry by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key of the item to be retrieved</param>
        /// <param name="item">Outputs null if the item is not found, or is of the incorrect type.</param>
        /// <returns>True if the entry is found, otherwise false</returns>
        bool TryGet<T>(string key, out T item);

        /// <summary>
        /// Removes all stored cache entries
        /// </summary>
        void Flush();

        /// <summary>
        /// Checks whether the specified key exists in the cache.
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if the key is known, otherwise false.</returns>
        bool HasKey(string key);
    }

    [Serializable]
    public class CacheItem<T>
    {
        /// <summary>
        /// Default .ctor for serialization and deserialization only
        /// </summary>
        public CacheItem()
        { }

        public CacheItem(T payload)
            : this(payload, TimeSpan.Zero)
        {
        }

        public CacheItem(T payload, TimeSpan lifespan)
        {
            Payload = payload;
            Expiry = lifespan != TimeSpan.Zero ? DateTime.UtcNow.Add(lifespan) : DateTime.MaxValue;
        }

        public T Payload { get; private set; }

        public DateTime Expiry { get; private set; }

        public virtual bool HasExpired
        {
            get { return DateTime.UtcNow > Expiry; }
        }
    }

    [Serializable]
    public class AccessCountingCacheItem<T> : CacheItem<T>
    {
        private int _accessCount = 0;

        public AccessCountingCacheItem(T payload, Times accessCount)
            : base(payload)
        {
            MaxAccessCount = accessCount;
        }

        public Times MaxAccessCount { get; private set; }

        public void Step()
        {
            _accessCount++;
        }

        public override bool HasExpired
        {
            get { return _accessCount >= MaxAccessCount.Value; }
        }
    }

    public class AccountCache
    {
        public string accountnumber { get; set; }
        public Integration.Enums.SupportedLanguage RequestLanguage { get; set; }
    }

    [Serializable]
    public struct Times
    {
        public int Value;

        private Times(int value)
        {
            Value = value;
        }

        public static Times Zero = new Times(0);

        public static Times Once = new Times(1);

        public static Times Max = new Times(int.MaxValue);

        public static Times Exactly(int value)
        {
            return new Times(value);
        }
    }
}