using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;
using CodeBucket.Bitbucket.Controllers.Followers;
using System.Threading.Tasks;

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

        protected override object OnUpdate(bool forced)
        {
            return Application.Client.Users[_owner].Repositories[_name].GetFollowers(forced).Followers.OrderBy(x => x.Username).ToList();
        }
    }
}

