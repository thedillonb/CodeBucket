using BitbucketSharp.Models;
using CodeBucket.Controllers;

namespace CodeBucket.ViewControllers
{
    public class RepoEventsViewController : EventsViewController
    {
        public RepoEventsViewController(string username, string slug)
            : base(username)
        {
            ReportRepository = false;
            Controller = new RepoEventsController(this, username, slug);
        }
    }
}

