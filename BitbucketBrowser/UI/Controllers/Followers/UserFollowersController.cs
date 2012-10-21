using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;

namespace BitbucketBrowser.UI.Controllers.Followers
{
    public class UserFollowersController : FollowersController
    {
        private readonly string _name;
        
        public UserFollowersController(string name)
        {
            _name = name;
        }
        
        protected override List<FollowerModel> OnUpdate(bool forced)
        {
            var f = Application.Client.Users[_name].GetFollowers(forced).Followers;
            return f.OrderBy(x => x.Username).ToList();
        }
    }
}

