using System;
using System.Linq;
using BitbucketSharp;
using System.Collections.Generic;
using MonoTouch;

namespace CodeBucket.Data
{
    public class WebCacheProvider : ICacheProvider
    {
        private const int MaxCachedItems = 50;
        private readonly Dictionary<string, CachedObject> _cache = new Dictionary<string, CachedObject>(MaxCachedItems);

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
            var cacheObj = new CachedObject { When = DateTime.Now, Cached = objectToCache };

            lock (_cache)
            {
                _cache[name] = cacheObj;

                if (_cache.Count >= MaxCachedItems)
                {
                    //Create a reverse dictionary
                    var sortedCached = new SortedDictionary<CachedObject, string>(new CachedObjectComparable());
                    foreach (var key in _cache.Keys)
                        sortedCached[_cache[key]] = key; 

                    //Remove the first 25 items
                    int i = 0;
                    foreach (var obj in sortedCached)
                    {
                        _cache.Remove(obj.Value);
                        Utilities.Log("Removed cached item {0} -> {1}", obj.Value, obj.Key.GetType().ToString());
                        i++;
                        if (i >= MaxCachedItems / 2)
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
                removeList.AddRange(_cache.Keys.Where(k => k.StartsWith(name)));

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

