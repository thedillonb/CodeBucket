using GitHubSharp.Models;
using System.Collections.Generic;

namespace CodeBucket.GitHub.Controllers.Repositories
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
            Title = "Repositories";
        }

        protected override List<RepositoryModel> GetData(bool force, int currentPage, out int nextPage)
        {
            var selected = 0;
            InvokeOnMainThread(() => { selected = _segment.SelectedSegment; });
            GitHubSharp.GitHubResponse<List<RepositoryModel>> data = null;

            //Set the show property based on what is selected
            ShowOwner = selected != 0;

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


