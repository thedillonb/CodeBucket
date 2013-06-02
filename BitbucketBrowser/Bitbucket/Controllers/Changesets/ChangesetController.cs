using System;
using BitbucketSharp.Models;
using CodeBucket.Controllers;
using CodeBucket.Elements;
using MonoTouch.Dialog;
using System.Collections.Generic;
using System.Linq;
using BitbucketBrowser.Controllers;
using MonoTouch;


namespace BitbucketBrowser.Controllers.Changesets
{
    public class ChangesetController : Controller<List<ChangesetModel>>
    {
        private const int RequestLimit = 30;
        private DateTime _lastUpdate = DateTime.MinValue;
        private string _lastNode;
        private LoadMoreElement _loadMore;

        public string User { get; private set; }

        public string Slug { get; private set; }

        public ChangesetController(string user, string slug)
            : base(true, true)
        {
            User = user;
            Slug = slug;
            Style = MonoTouch.UIKit.UITableViewStyle.Plain;
            Title = "Changes";
            Root.UnevenRows = true;
        }

        protected virtual ChangesetsModel OnGetData(string startNode = null)
        {
            return Application.Client.Users[User].Repositories[Slug].Changesets.GetChangesets(RequestLimit, startNode);
        }

        private void GetMore()
        {
            this.DoWorkNoHud(() => {
                var moreEvents = OnGetData(_lastNode);
                var newChanges =
                    (from s in moreEvents.Changesets
                     orderby (s.Utctimestamp) descending
                     select s).ToList();
                
                //Always remove the first node since it should already be listed...
                if (newChanges.Count > 0)
                    newChanges.RemoveAt(0);
                
                //Save the last node...
                if (newChanges.Count > 0)
                {
                    AddItems(newChanges, false);
                    _lastNode = newChanges.Last().Node;
                }
                
                //Should never happen. Sanity check..
                if (_loadMore != null && newChanges.Count == 0)
                {
                    InvokeOnMainThread(() => {
                        Root.Remove(_loadMore.Parent as Section);
                        _loadMore.Dispose();
                        _loadMore = null;
                    });
                }
            }, 
            ex => Utilities.ShowAlert("Failure to load!", "Unable to load additional enries! " + ex.Message),
            () => {
                if (_loadMore != null)
                    _loadMore.Animating = false;
            });
        }

        protected override void OnRefresh ()
        {
            AddItems(Model);
        }

        private void AddItems(List<ChangesetModel> changes, bool prepend = true)
        {
            if (changes.Count == 0)
                return;
            
            var sec = new Section();
            changes.ForEach(x => {
                var desc = (x.Message ?? "").Replace("\n", " ").Trim();
                var el = new NameTimeStringElement { Name = x.Author, Time = (x.Utctimestamp), String = desc, Lines = 4 };
                el.Tapped += () => NavigationController.PushViewController(new ChangesetInfoController(User, Slug, x.Node), true);
                sec.Add(el);
            });

            if (sec.Count == 0)
                return;

            InvokeOnMainThread(delegate {
                if (Root.Count == 0)
                {
                    var r = new RootElement(Title) { UnevenRows = true };
                    r.Add(sec);
                    
                    //If there are more items to load then insert the load object
                    _loadMore = new PaginateElement("Load More", "Loading...", e => GetMore());
                    r.Add(new Section { _loadMore });
                    Root = r;
                }
                else
                {
                    if (prepend)
                        Root.Insert(0, sec);
                    else
                        Root.Insert(Root.Count - 1, sec);
                }
            }); 
        }

        protected override List<ChangesetModel> OnUpdate(bool forced)
        {
            var changes = OnGetData();
            var newChanges =
                         (from s in changes.Changesets
                          where (s.Utctimestamp) > _lastUpdate
                          orderby (s.Utctimestamp) descending
                          select s).ToList();
            if (newChanges.Count > 0)
                _lastUpdate = (from r in newChanges select (r.Utctimestamp)).Max();

            if (_lastNode == null)
            {
                //Get the last node we've visited!
                _lastNode = newChanges.Last().Node;
            }

            return newChanges;
        }
    }
}

