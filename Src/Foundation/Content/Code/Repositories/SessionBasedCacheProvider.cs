using DEWAXP.Foundation.DI;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace DEWAXP.Foundation.Content.Repositories
{
    //[Service(typeof(ICacheProvider), Lifetime = Lifetime.Singleton)]
    public class SessionBasedCacheProvider : ICacheProvider
    {
        private readonly Func<HttpSessionState> _getSession;

        public SessionBasedCacheProvider(Func<HttpSessionState> getSession)
        {
            _getSession = getSession;
        }

        private IEnumerable<string> Keys
        {
            get
            {
                var session = _getSession();
                if (session != null)
                {
                    foreach (var key in session.Keys)
                    {
                        yield return key.ToString();
                    }
                }
            }
        }

        public void Store<T>(string key, T item)
        {
            Store(key, new CacheItem<T>(item));
        }

        public void Store<T>(string key, CacheItem<T> item)
        {
            var session = _getSession();
            if (session != null)
            {
                session[key] = item;
            }
        }

        public void Remove(string key)
        {
            var session = _getSession();
            if (session != null)
            {
                session.Remove(key);
            }
        }

        public bool TryGet<T>(string key, out T item)
        {
            item = default(T);

            var session = _getSession();
            if (session != null && Keys.Contains(key))
            {
                var cacheEntry = session[key] != null ? session[key] as CacheItem<T> : null;
                if (cacheEntry != null)
                {
                    if (cacheEntry.HasExpired)
                    {
                        Remove(key);

                        return false;
                    }

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
            }
            return false;
        }

        public void Flush()
        {
            var session = _getSession();
            if (session != null)
            {
                session.RemoveAll();
            }
        }

        public bool HasKey(string key)
        {
            return Keys.Contains(key);
        }

        ~SessionBasedCacheProvider()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _disposed = true;

                Flush();
            }
        }
    }


    public class SessionCacheRegister: IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ICacheProvider, SessionBasedCacheProvider>(_getSession=> new SessionBasedCacheProvider(()=> HttpContext.Current.Session));
        }
    }
}