using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeFramework.Controllers;

namespace CodeBucket.Controllers
{
    public class RepoFollowersController : ListController<FollowerModel>
    {
        private readonly string _name;
        private readonly string _owner;
        
        public RepoFollowersController(IView<ListModel<FollowerModel>> view, string owner, string name)
            : base(view)
        {
            _name = name;
            _owner = owner;
        }

        public override void Update(bool force)
        {
            Model = new ListModel<FollowerModel> {
                Data = Application.Client.Users[_owner].Repositories[_name].GetFollowers(force).Followers.OrderBy(x => x.Username).ToList()
            };
        }
    }
}

