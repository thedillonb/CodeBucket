using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;
using CodeBucket.Bitbucket.Controllers.Followers;

namespace CodeBucket.Bitbucket.Controllers.Followers
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

        protected override List<FollowerModel> GetData(bool force, int currentPage, out int nextPage)
        {
            var f = Application.Client.Users[_owner].Repositories[_name].GetFollowers(force).Followers;
            nextPage = -1;
            return f.OrderBy(x => x.Username).ToList();
        }
    }
}

