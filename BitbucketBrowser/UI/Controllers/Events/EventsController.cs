using System;
using System.Linq;
using System.Threading;
using BitbucketSharp;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Collections.Generic;
using CodeFramework.UI.Controllers;
using CodeFramework.UI.Elements;
using BitbucketBrowser.UI.Controllers.Wikis;
using BitbucketBrowser.UI.Controllers.Repositories;
using BitbucketBrowser.UI.Controllers.Issues;

namespace BitbucketBrowser.UI.Controllers.Events
{
    public class EventsController : Controller<List<EventModel>>
    {
        private DateTime _lastUpdate = DateTime.MinValue;
        private int _firstIndex = 0;
        private int _lastIndex = 0;
        private LoadMoreElement _loadMore;

        public string Username { get; private set; }

        public bool ReportRepository { get; set; }

        public EventsController(string username, bool push = true) 
            : base(push, true)
        {
            Title = "Events";
            Style = UITableViewStyle.Plain;
            Username = username;
            Root.UnevenRows = true;
            ReportRepository = false;
        }

        protected virtual EventsModel OnGetData(int start = 0, int limit = 30)
        {
            return Application.Client.Users[Username].GetEvents(start, limit);
        }

        private void GetMore()
        {
            ThreadPool.QueueUserWorkItem(delegate {
                var currentCount = OnGetData(0, 0).Count;
                var moreEvents = OnGetData(currentCount - _firstIndex + _lastIndex);
                _firstIndex = currentCount;
                _lastIndex += moreEvents.Events.Count;
                var newEvents = moreEvents.Events;
                AddItems(newEvents, false);

                //Should never happen. Sanity check..
                if (_loadMore != null)
                {
                    BeginInvokeOnMainThread(() => {
                        _loadMore.Animating = false;

                        if (_firstIndex == _lastIndex)
                        {
                            Root.Remove(_loadMore.Parent as Section);
                            _loadMore.Dispose();
                            _loadMore = null;
                        }
                    });
                }
            });
        }

        protected override List<EventModel> OnUpdate()
        {
            var events = OnGetData();
            _firstIndex = events.Count;
            _lastIndex = events.Events.Count;

             var newEvents =
                 (from s in events.Events
                  where DateTime.Parse(s.UtcCreatedOn) > _lastUpdate
                  orderby DateTime.Parse(s.UtcCreatedOn) descending
                  select s).ToList();
             if (newEvents.Count > 0)
                 _lastUpdate = (from r in newEvents select DateTime.Parse(r.UtcCreatedOn)).Max();
            return newEvents;
        }

        protected override void OnRefresh()
        {
            AddItems(Model);
        }

        private void AddItems(List<EventModel> events, bool prepend = true)
        {
            if (events.Count == 0)
                return;

            var sec = new Section();
            events.ForEach(e => {
                if (!NewsFeedElement.SupportedEvents.Contains(e.Event))
                    return;

                var newsEl = new NewsFeedElement(e, ReportRepository);
                if (e.Event == EventModel.Type.Commit) 
                {
                    newsEl.Tapped += () => { 
                        NavigationController.PushViewController(
                            new ChangesetInfoController(e.Repository.Owner, e.Repository.Slug, e.Node)
                            { Repo = e.Repository }, true);
                    };
                } 
                else if (e.Event == EventModel.Type.WikiCreated || e.Event == EventModel.Type.WikiUpdated) 
                {
                    newsEl.Tapped += () => NavigationController.PushViewController(new WikiInfoController(e.Repository.Owner, e.Repository.Slug, e.Description), true);
                }
                else if (e.Event == EventModel.Type.CreateRepo || e.Event == EventModel.Type.StartFollowRepo || e.Event == EventModel.Type.StopFollowRepo)
                {
                    newsEl.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(e.Repository), true);
                }
                else if (e.Event == EventModel.Type.IssueComment || e.Event == EventModel.Type.IssueUpdated || e.Event == EventModel.Type.IssueReported)
                {
                    newsEl.Tapped += () => NavigationController.PushViewController(new IssuesController(e.Repository.Owner, e.Repository.Slug), true);
                }

                sec.Add(newsEl);
            });

            if (sec.Count == 0)
                return;

            InvokeOnMainThread(delegate {
                if (Root.Count == 0)
                {
                    var r = new RootElement(Title) { sec };

                    //If there are more items to load then insert the load object
                    if (_lastIndex != _firstIndex)
                    {
                        _loadMore = new PaginateElement("Load More", "Loading...", (e) => GetMore());
                        r.Add(new Section() { _loadMore });
                    }

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
    }
}