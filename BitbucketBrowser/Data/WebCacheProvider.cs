using System;
using BitbucketSharp;
using System.Linq;
using System.Collections.Generic;

namespace BitbucketBrowser.Data
{
    public class WebCacheProvider : ICacheProvider
    {
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

        public T Get<T>(string name) where T : class
        {
            return Get<T>(name, DefaultDuractionInMinutes);
        }

        public T Get<T>(string name, int cacheDurationInMinutes) where T : class
        {
            lock (_cache)
            {
                if (!IsCached(name))
                    return null;

                var cached = _cache[name] as CachedObject<T>;
                if (cached == null)
                    return null;

                if (cached.When.AddMinutes(cacheDurationInMinutes) < DateTime.Now)
                    return null;

                return cached.Cached;
            }
        }

        public bool IsCached(string name)
        {
            lock (_cache)
            {
                return _cache.ContainsKey(name);
            }
        }

        public void Set<T>(T objectToCache, string name) where T : class
        {
            var cacheObj = new CachedObject<T>() { When = DateTime.Now, Cached = objectToCache };
            _cache.Add(name, cacheObj);
        }

        public void Delete(string name)
        {
            lock (_cache)
            {
                _cache.Remove(name);
            }
        }

        public void DeleteWhereStartingWith(string name)
        {
            lock (_cache)
            {
                var removeList = new List<string>(_cache.Keys.Count);
                foreach (var k in _cache.Keys)
                {
                    if (k.StartsWith(name))
                        removeList.Add(k);
                }

                foreach (var item in removeList)
                {
                    _cache.Remove(item);
                }
            }
        }

        public void DeleteAll()
        {
            lock (_cache)
            {
                _cache.Clear();
            }
        }

        public int DefaultDuractionInMinutes {
            get { return 10; }
        }
    }
}

