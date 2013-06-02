using System.Linq;
using GitHubSharp.Models;
using System.Collections.Generic;
using CodeBucket.Data;
using CodeBucket.GitHub.Controllers.Followers;
using CodeBucket;

namespace CodeBucket.GitHub.Controllers.Followers
{
    public class RepoFollowersController : FollowersController
    {
        private readonly string _name;
        private readonly string _owner;
        
        public RepoFollowersController(string owner, string name)
        {
            _name = name;
            _owner = owner;
        }

        protected override List<BasicUserModel> GetData(bool force, int currentPage, out int nextPage)
        {
            var f = Application.GitHubClient.API.GetRepositoryWatchers(_owner, _name, currentPage);
            nextPage = f.Next == null ? -1 : currentPage + 1;
            return f.Data.OrderBy(x => x.Login).ToList();
        }
    }
}
