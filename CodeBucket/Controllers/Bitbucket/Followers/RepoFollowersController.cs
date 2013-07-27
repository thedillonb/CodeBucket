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

        protected override async Task DoRefresh(bool force)
        {
            if (Model == null || force)
                await Task.Run(() => { Model = Application.Client.Users[_owner].Repositories[_name].GetFollowers(force).Followers.OrderBy(x => x.Username).ToList(); });
            AddItems<FollowerModel>(Model, CreateElement);
        }
    }
}

