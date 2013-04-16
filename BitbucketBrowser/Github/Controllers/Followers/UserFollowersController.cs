using System.Collections.Generic;
using System.Linq;
using BitbucketBrowser.Data;
using GitHubSharp.Models;

namespace BitbucketBrowser.GitHub.Controllers.Followers
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
