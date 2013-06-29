using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;

namespace CodeBucket.Bitbucket.Controllers.Followers
{
    public class UserFollowersController : FollowersController
    {
        private readonly string _name;
        
        public UserFollowersController(string name)
        {
            _name = name;
        }

        protected override List<FollowerModel> GetData(bool force, int currentPage, out int nextPage)
        {
            var f = Application.Client.Users[_name].GetFollowers(force).Followers;
            nextPage = -1;
            return f.OrderBy(x => x.Username).ToList();
        }
    }
}

