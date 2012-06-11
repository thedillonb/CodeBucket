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
            var client = new Client("thedillonb", "djames");
            return client.Users[Username].Repositories[Slug].GetEvents();
        }
    }

    public class EventsController : Controller<List<EventModel>>
    {
        private DateTime _lastUpdate = DateTime.MinValue;

        public string Username { get; private set; }

        public bool ReportUser { get; set; }

        public EventsController(string username, bool push = true) 
            : base(push, true)
        {
            Title = "Events";
            Style = UITableViewStyle.Plain;
            Username = username;
            Root.UnevenRows = true;
            Root.Add(new Section());
            ReportUser = true;
        }

        protected virtual EventsModel OnGetData()
        {
            var client = new Client("thedillonb", "djames");
            return client.Users[Username].GetEvents();
        }

        protected override List<EventModel> OnUpdate()
        {
            var events = OnGetData();

             var newEvents =
                 (from s in events.Events
                  where DateTime.Parse(s.CreatedOn) > _lastUpdate
                  orderby DateTime.Parse(s.CreatedOn)
                  select s).ToList();
             if (newEvents.Count > 0)
                 _lastUpdate = (from r in newEvents select DateTime.Parse(r.CreatedOn)).Max();
            return newEvents;
        }

        protected override void OnRefresh()
        {
            InvokeOnMainThread(delegate {
                Model.ForEach(e => {
                    var newsEl = new NewsFeedElement(e) { ReportUser = ReportUser };
                    newsEl.Tapped += () => {
                        if (e.Event == "commit") {
                            NavigationController.PushViewController(new ChangesetInfoController(e.Repository.Owner, e.Repository.Slug, e.Node), true);
                        }
                    };
                    Root[0].Insert(0, UITableViewRowAnimation.Top, newsEl);
                });
            });

        }
    }
}