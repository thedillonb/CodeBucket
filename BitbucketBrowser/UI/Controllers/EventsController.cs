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
    public class EventsController : Controller<List<EventModel>>
    {
        private DateTime _lastUpdate = DateTime.MinValue;

        public string Username { get; private set; }

        public EventsController(string username, bool push = true) 
            : base(push, true)
        {
            Title = "Events";
            Style = UITableViewStyle.Plain;
            Username = username;
            Root.UnevenRows = true;
            Root.Add(new Section());
        }

        protected override List<EventModel> OnUpdate()
        {
            var client = new Client("thedillonb", "djames");
            EventsModel events = client.Users[Username].GetEvents();

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
                Model.ForEach(e => Root[0].Insert(0, UITableViewRowAnimation.Top, new NewsFeedElement(e)));
            });
        }
    }
}