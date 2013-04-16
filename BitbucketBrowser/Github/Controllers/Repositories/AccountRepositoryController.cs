using BitbucketBrowser.Data;
using GitHubSharp.Models;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Linq;
using System.Collections.Generic;
using BitbucketBrowser.Elements;

namespace BitbucketBrowser.GitHub.Controllers.Repositories
{
    public class AccountRepositoryController : RepositoryController
    {
        private const string SavedSelection = "REPO_SELECTION";
        private static string[] _sections = new [] { "Owned", "Watched", "Starred" };
        
        public AccountRepositoryController(string username)
            : base(username)
        {
            MultipleSelections = _sections;
            MultipleSelectionsKey = SavedSelection;
        }

        protected override List<RepositoryModel> GetData(bool force, int currentPage, out int nextPage)
        {
            var selected = 0;
            InvokeOnMainThread(() => { selected = _segment.SelectedSegment; });
            GitHubSharp.GitHubResponse<List<RepositoryModel>> data = null;
            
            if (selected == 0)
                data = Application.GitHubClient.API.ListRepositories(Username);
            else if (selected == 1)
                data = Application.GitHubClient.API.GetRepositoriesWatching(Username);
            else if (selected == 2)
                data = Application.GitHubClient.API.GetRepositoriesStarred();
            else
            {
                nextPage = -1;
                return new List<RepositoryModel>();
            }
            
            nextPage = data.Next == null ? -1 : currentPage + 1;
            return data.Data;
        }
    }
}


