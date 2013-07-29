using CodeBucket.Bitbucket.Controllers.Accounts;
using CodeBucket.Bitbucket.Controllers;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using BitbucketSharp.Models;
using System.Linq;
using System.Collections.Generic;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using System.Threading.Tasks;

namespace CodeBucket.Bitbucket.Controllers.Followers
{
    public abstract class FollowersController : ModelDrivenController
    {
		protected FollowersController()
			: base(typeof(List<FollowerModel>))
		{
            Title = "Followers";
            SearchPlaceholder = "Search Followers";
            Style = UITableViewStyle.Plain;
		}

        protected Element CreateElement(FollowerModel s)
        {
            StyledElement sse = new UserElement(s.Username, s.FirstName, s.LastName, s.Avatar);
            sse.Tapped += () => NavigationController.PushViewController(new ProfileController(s.Username), true);
            return sse;
        }

        protected override void OnRefresh()
        {
            AddItems<FollowerModel>(Model as List<FollowerModel>, CreateElement, "No Followers");
        }
	}
}

