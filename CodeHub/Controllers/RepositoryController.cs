using System.Threading;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using CodeFramework.UI.Controllers;
using GithubSharp.Core.Models;
using System;
using GithubSharp.Core.Services;

namespace CodeHub.Controllers.Repositories
{
    public class RepositoryController : Controller<List<Repository>>
    {
        public string Username { get; private set; }
        public bool ShowOwner { get; set; }

        public RepositoryController(string username, bool push = true) 
            : base(push, true)
        {
            Title = "Repositories";
            Style = UITableViewStyle.Plain;
            Username = username;
            AutoHideSearch = true;
            EnableSearch = true;
            ShowOwner = true;
        } 

        protected override void OnRefresh()
        {
            if (Model.Count == 0)
                return;

            var sec = new Section();
            Model.ForEach(x => {
                RepositoryElement sse = new RepositoryElement(x) { ShowOwner = ShowOwner };
                //sse.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(x), true);
                sec.Add(sse);
            });

            //Sort them by name
            sec.Elements = sec.Elements.OrderBy(x => ((RepositoryElement)x).Model.Name).ToList();

            InvokeOnMainThread(delegate {
                var root = new RootElement(Title) { sec };
                Root = root;
            });
        }

        protected override List<Repository> OnUpdate()
        {
            var repos = new GithubSharp.Core.API.Repository(new You(), new Fuck());
            return new List<Repository>(repos.List("xamarin"));
        }
    }

    public class You : GithubSharp.Core.Services.ICacheProvider
    {
        [ThreadStatic]
        private static Dictionary<string, object> _cache;
        
        private const string CachePrefix = "GithubSharp.Plugins.CacheProviders.BasicCacher";
        
        public You()
        {
            _cache = new Dictionary<string, object>();
        }
        
        public T Get<T>(string Name) where T : class
        {
            return Get<T>(Name, DefaultDuractionInMinutes);
        }
        
        public T Get<T>(string Name, int CacheDurationInMinutes) where T : class
        {
            if (!_cache.ContainsKey(CachePrefix + Name)) return null;
            var cached = _cache[CachePrefix + Name] as CachedObject<T>;
            if (cached == null) return null;
            
            if (cached.When.AddMinutes(CacheDurationInMinutes) < DateTime.Now)
                return null;
            
            return cached.Cached;
        }
        
        public bool IsCached<T>(string Name) where T : class
        {
            return _cache.ContainsKey(CachePrefix + Name);
        }
        
        public void Set<T>(T ObjectToCache, string Name) where T : class
        {
            var cacheObj = new CachedObject<T>();
            cacheObj.Cached = ObjectToCache;
            cacheObj.When = DateTime.Now;
            
            _cache[CachePrefix + Name] = cacheObj;
        }
        
        public void Delete(string Name)
        {
            _cache.Remove(CachePrefix + Name);
        }
        
        public void DeleteWhereStartingWith(string Name)
        {
            var enumerator = _cache.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Key.StartsWith(CachePrefix + Name))
                    _cache.Remove(enumerator.Current.Key);
            }
        }
        
        public void DeleteAll<T>() where T : class
        {
            _cache.Clear();
        }
        
        public int DefaultDuractionInMinutes
        {
            get { return 20; }
        }
    }

    public class Fuck : GithubSharp.Core.Services.ILogProvider
    {
        #region ILogProvider implementation

        public void LogWarning(string Message, params object[] Arguments)
        {

        }

        #endregion

        #region ILogProvider implementation
        public void LogMessage(string Message, params object[] Arguments)
        {

        }
        public bool HandleAndReturnIfToThrowError(System.Exception error)
        {
            return false;
        }
        public bool DebugMode {
            get;
            set;
        }
        #endregion
    }
}