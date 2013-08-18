using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeFramework.Controllers;

namespace CodeBucket.Controllers
{
    public class UserFollowersController : ListController<FollowerModel>
    {
        private readonly string _name;
        
        public UserFollowersController(IView<ListModel<FollowerModel>> view, string name)
            : base(view)
        {
            _name = name;
        }

        public override void Update(bool force)
        {
            Model = new ListModel<FollowerModel> {
                Data = Application.Client.Users[_name].GetFollowers(force).Followers.OrderBy(x => x.Username).ToList()
            };
        }
    }
}

