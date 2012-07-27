using System;
using System.Linq;
using System.Threading;
using BitbucketSharp;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace BitbucketBrowser.UI
{
    public class RepoEventsController : EventsController
    {
        public string Slug { get; private set; }

        public RepoEventsController(string username, string slug)
            : base(username)
        {
            Slug = slug;
        }

        protected override EventsModel OnGetData()
        {
            return Application.Client.Users[Username].Repositories[Slug].GetEvents();
        }
    }

    public class EventsController : Controller<List<EventModel>>
    {
        private DateTime _lastUpdate = DateTime.MinValue;

        public string Username { get; private set; }

        public bool ReportUser { get; set; }

        public bool ReportRepository { get; set; }

        public EventsController(string username, bool push = true) 
            : base(push, true)
        {
            Title = "Events";
            Style = UITableViewStyle.Plain;
            Username = username;
            Root.UnevenRows = true;
            ReportUser = true;
            ReportRepository = false;
        }

        protected virtual EventsModel OnGetData()
        {
            return Application.Client.Users[Username].GetEvents(0, 40);
        }

        protected override List<EventModel> OnUpdate()
        {
            var events = OnGetData();

             var newEvents =
                 (from s in events.Events
                  where DateTime.Parse(s.UtcCreatedOn) > _lastUpdate
                  //orderby DateTime.Parse(s.UtcCreatedOn)
                  select s).ToList();
             if (newEvents.Count > 0)
                 _lastUpdate = (from r in newEvents select DateTime.Parse(r.CreatedOn)).Max();
            return newEvents;
        }

        protected override void OnRefresh()
        {
            if (Model.Count == 0)
                return;

            var sec = new Section();
            Model.ForEach(e => {
                if (!NewsFeedElement.SupportedEvents.Contains(e.Event))
                    return;

                var newsEl = new NewsFeedElement(e) { ReportUser = ReportUser, ReportRepository = ReportRepository };
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

                sec.Add(newsEl);
            });

            if (sec.Count == 0)
                return;

            InvokeOnMainThread(delegate {
                if (Root.Count == 0)
                {
                    var r = new RootElement(Title) { sec };
                    Root = r;
                }
                else
                    Root.Insert(0, sec);
            });

        }
    }
}