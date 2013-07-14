using BitbucketSharp.Models;
using CodeBucket.Bitbucket.Controllers.Events;

namespace CodeBucket.Bitbucket.Controllers.Events
{
    public class RepoEventsController : EventsController
    {
        public string Slug { get; private set; }
        
        public RepoEventsController(string username, string slug)
            : base(username)
        {
            Slug = slug;
            ReportRepository = false;
        }
        
        protected override EventsModel OnGetData(int start = 0, int limit = 30)
        {
            return Application.Client.Users[Username].Repositories[Slug].GetEvents(start, limit);
        }
    }
}

