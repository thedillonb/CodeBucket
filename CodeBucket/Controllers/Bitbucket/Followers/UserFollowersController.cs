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

        protected override async Task DoRefresh(bool force)
        {
            if (Model == null || force)
                await Task.Run(() => { Model = Application.Client.Users[_name].GetFollowers(force).Followers.OrderBy(x => x.Username).ToList(); });
            AddItems<FollowerModel>(Model, CreateElement);
        }
    }
}

