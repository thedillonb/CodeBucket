using BitbucketSharp.Models;

namespace BitbucketBrowser.Controllers.Events
{
    public class RepoEventsController : EventsController
    {
        public string Slug { get; private set; }
        
        public RepoEventsController(string username, string slug)
            : base(username)
        {
            Slug = slug;
        }
        
        protected override EventsModel OnGetData(int start = 0, int limit = 30)
        {
            return Application.Client.Users[Username].Repositories[Slug].GetEvents(start, limit);
        }
    }
}

