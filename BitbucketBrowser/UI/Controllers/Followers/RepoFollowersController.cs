using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;

namespace BitbucketBrowser.UI.Controllers.Followers
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

        protected override List<FollowerModel> OnUpdate(bool forced)
        {
            var f = Application.Client.Users[_owner].Repositories[_name].GetFollowers(forced).Followers;
            return f.OrderBy(x => x.Username).ToList();
        }
    }
}

