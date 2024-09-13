using DEWAXP.Foundation.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace DEWAXP.Foundation.Content.Repositories
{
    [Service(Lifetime = Lifetime.Transient)]
    public static class ApplicationCacheProvider
    {
        private static Cache _cache;
        static ApplicationCacheProvider()
        {
            _cache = _cache == null ? new Cache() : _cache;

        }

        public static void Store(string key, object item)
        {
            _cache.Insert(key, item, null, DateTime.UtcNow.AddHours(2), Cache.NoSlidingExpiration);
        }

        public static void Remove(string key)
        {

            if (_cache[key] != null)
            {
                _cache.Remove(key);
            }
        }

        public static bool TryGet<T>(string key, out T item)
        {
            item = default(T);

            var i = _cache[key];
            if (i != null)
            {
                item = (T)i;

                return true;

            }
            return false;
        }
    }
}