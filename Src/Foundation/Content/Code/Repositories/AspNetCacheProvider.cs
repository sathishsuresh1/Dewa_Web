using DEWAXP.Foundation.DI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace DEWAXP.Foundation.Content.Repositories
{
    [Service(Lifetime = Lifetime.Transient)]
    public class AspNetCacheProvider : ICacheProvider
    {
	    private readonly Cache _cache;
	    private readonly Func<string, string> _createUniqueKey;
	    private readonly Func<TimeSpan> _getSlidingExpiration;

	    /// <summary>
	    /// Default .ctor
	    /// </summary>
	    /// <param name="createUniqueKey">Gets a unique value to disambiguate duplicate keys</param>
	    /// <param name="getSlidingExpiration">Gets the sliding expiration to use when "indefinitely" stored items are added to the cache.</param>
	    public AspNetCacheProvider(Func<string, string> createUniqueKey, Func<TimeSpan> getSlidingExpiration)
		{
			_createUniqueKey = createUniqueKey;
		    _getSlidingExpiration = getSlidingExpiration;
		    _cache = HttpContext.Current.Cache;
		}

	    private IEnumerable<string> Keys
        {
            get
            {
                var @enum = _cache.GetEnumerator();
                while (@enum.MoveNext())
                {
					yield return ((DictionaryEntry)@enum.Current).Key.ToString();
				}
            }
        }

	    public void Store<T>(string key, T item)
	    {
		    Store(key, new CacheItem<T>(item));
	    }

	    public void Store<T>(string key, CacheItem<T> item)
        {
	        var qualifiedKey = _createUniqueKey(key);
	        if (Keys.Contains(qualifiedKey))
			{
				Remove(qualifiedKey);
	        }

	        var absExpiry = item.Expiry != DateTime.MaxValue ? item.Expiry : Cache.NoAbsoluteExpiration;
	        var slidingExpiration = item.Expiry != DateTime.MaxValue ? Cache.NoSlidingExpiration : _getSlidingExpiration();

			_cache.Add(qualifiedKey, item, null, absExpiry, slidingExpiration, CacheItemPriority.Normal, null);
		}

        public void Remove(string key)
        {
			var qualifiedKey = _createUniqueKey(key);

			_cache.Remove(qualifiedKey);
        }

        public bool TryGet<T>(string key, out T item)
        {
            item = default(T);

	        var qualifiedKey = _createUniqueKey(key);
			var cacheEntry = _cache[qualifiedKey] as CacheItem<T>;
            if (cacheEntry != null)
            {
                if (cacheEntry is AccessCountingCacheItem<T>)
                {
                    ((AccessCountingCacheItem<T>)cacheEntry).Step();

                    if (cacheEntry.HasExpired)
                    {
                        Remove(key);
                    }
                }
                item = cacheEntry.Payload;
                return true;
            }
            return false;
        }

        public void Flush()
        {
            var @enum = _cache.GetEnumerator();
            while (@enum.MoveNext())
            {
                _cache.Remove(@enum.Current.ToString());
            }
        }

	    public bool HasKey(string key)
	    {
			var qualifiedKey = _createUniqueKey(key);

		    return Keys.Contains(qualifiedKey);
	    }

	    ~AspNetCacheProvider()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private readonly bool _disposed;
		private void Dispose(bool disposing)
		{
			if (_disposed) return;

			if (disposing)
			{
				Flush();
			}
		}
	}
}