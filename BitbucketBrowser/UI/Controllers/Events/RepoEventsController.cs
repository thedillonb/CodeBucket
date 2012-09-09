using System;
using BitbucketSharp.Models;

namespace BitbucketBrowser.UI.Controllers.Events
{
    public class RepoEventsController : EventsController
    {
        public string Slug { get; private set; }
        
        public RepoEventsController(string username, string slug)
            : base(username)
        {
            Slug = slug;
        }
        
        protected override EventsModel OnGetData(int start, int limit)
        {
            return Application.Client.Users[Username].Repositories[Slug].GetEvents(start, limit);
        }
    }
}

