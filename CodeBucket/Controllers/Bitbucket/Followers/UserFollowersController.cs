using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeBucket.Bitbucket.Controllers.Followers
{
    public class UserFollowersController : FollowersController
    {
        private readonly string _name;
        
        public UserFollowersController(string name)
        {
            _name = name;
        }

        protected override object OnUpdate(bool forced)
        {
            return Application.Client.Users[_name].GetFollowers(forced).Followers.OrderBy(x => x.Username).ToList();
        }
    }
}

