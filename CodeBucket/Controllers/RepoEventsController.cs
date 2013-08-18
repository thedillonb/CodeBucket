using System;
using System.Linq;
using BitbucketSharp.Models;
using CodeBucket.Bitbucket.Controllers;
using System.Collections.Generic;
using CodeBucket.Bitbucket.Controllers.Issues;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using CodeBucket.ViewControllers;

namespace CodeBucket.Controllers
{
    public class RepoEventsController : EventsController
    {
        public string Slug { get; private set; }

        public RepoEventsController(IListView<EventModel> view, string username, string slug)
            : base(view, username)
        {
            Slug = slug;
        }

        protected override List<EventModel> GetData(int start = 0, int limit = 30)
        {
            var events = Application.Client.Users[Username].Repositories[Slug].GetEvents(start, limit);
            return events.Events.OrderByDescending(x => x.UtcCreatedOn).ToList();
        }

        protected override int GetTotalItemCount()
        {
            return Application.Client.Users[Username].Repositories[Slug].GetEvents(0, 0).Count;
        }
    }
}