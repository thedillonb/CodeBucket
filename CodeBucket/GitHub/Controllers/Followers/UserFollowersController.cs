using System.Collections.Generic;
using System.Linq;
using CodeBucket.Data;
using GitHubSharp.Models;
using CodeBucket.GitHub.Controllers.Followers;
using CodeBucket;

namespace CodeBucket.GitHub.Controllers.Followers
{
    public class UserFollowersController : FollowersController
    {
        private readonly string _name;
        
        public UserFollowersController(string name)
        {
            _name = name;
        }
        
        protected override List<BasicUserModel> GetData(bool force, int currentPage, out int nextPage)
        {
            var f = Application.GitHubClient.API.GetUserFollowers(_name, currentPage);
            nextPage = f.Next == null ? -1 : currentPage + 1;
            return f.Data.OrderBy(x => x.Login).ToList();
        }
    }
}
