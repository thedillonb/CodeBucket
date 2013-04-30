using System;
using BitbucketSharp;
using System.Linq;
using System.Collections.Generic;
using MonoTouch;

namespace BitbucketBrowser.Data
{
    public class WebCacheProvider : BitbucketSharp.ICacheProvider, GitHubSharp.ICacheProvider
    {
        private static int MAX_CACHED_ITEMS = 50;
        private readonly Dictionary<string, CachedObject> _cache = new Dictionary<string, CachedObject>(MAX_CACHED_ITEMS);

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

                var cached = _cache[name];
                if (cached == null)
                    return null;

                if (cached.When.AddMinutes(cacheDurationInMinutes) < DateTime.Now)
                    return null;

                return cached.Cached as T;
            }
        }

        public bool IsCached(string name)
        {
            lock (_cache)
            {
                return _cache.ContainsKey(name);
            }
        }

        private class CachedObjectComparable : IComparer<CachedObject>
        {
            public int Compare(CachedObject x, CachedObject y)
            {
                return x.When.CompareTo(y.When);
            }
        }

        public void Set<T>(T objectToCache, string name) where T : class
        {
            var cacheObj = new CachedObject() { When = DateTime.Now, Cached = objectToCache };

            lock (_cache)
            {
                _cache[name] = cacheObj;

                if (_cache.Count >= MAX_CACHED_ITEMS)
                {
                    //Create a reverse dictionary
                    var sortedCached = new SortedDictionary<CachedObject, string>(new CachedObjectComparable());
                    foreach (var key in _cache.Keys)
                        sortedCached[_cache[key] as CachedObject] = key; 

                    //Remove the first 25 items
                    int i = 0;
                    foreach (var obj in sortedCached)
                    {
                        _cache.Remove(obj.Value);
                        Utilities.Log("Removed cached item {0} -> {1}", obj.Value, obj.Key.GetType().ToString());
                        i++;
                        if (i >= MAX_CACHED_ITEMS / 2)
                            break;
                    }
                }
            }
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

