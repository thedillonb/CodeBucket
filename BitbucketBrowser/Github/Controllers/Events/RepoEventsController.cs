using System.Collections.Generic;
using BitbucketBrowser.Data;
using GitHubSharp.Models;

namespace BitbucketBrowser.GitHub.Controllers.Events
{
    public class RepoEventsController : EventsController
    {
        public string Slug { get; private set; }
        
        public RepoEventsController(string username, string slug)
            : base(username)
        {
            Slug = slug;
        }
        
        protected override List<EventModel> OnGetData(int page = 1)
        {
            var response = Application.GitHubClient.API.GetRepositoryEvents(Username, Slug, page);
            if (response.Next != null)
                _nextPage = page + 1;
            else
                _nextPage = -1;
            return response.Data;
        }
    }
}
