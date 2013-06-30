using CodeBucket.Bitbucket.Controllers.Accounts;
using CodeBucket.Bitbucket.Controllers;
using CodeBucket.Elements;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using BitbucketSharp.Models;
using System.Linq;
using System.Collections.Generic;
using CodeBucket.Controllers;

namespace CodeBucket.Bitbucket.Controllers.Followers
{
    public abstract class FollowersController : ListController<FollowerModel>
    {
		protected FollowersController()
			: base(true)
		{
            Title = "Followers";
            SearchPlaceholder = "Search Followers";
		}

        protected override Element CreateElement(FollowerModel s)
        {
            StyledElement sse = new UserElement(s.Username, s.FirstName, s.LastName, s.Avatar);
            sse.Tapped += () => NavigationController.PushViewController(new ProfileController(s.Username), true);
            return sse;
        }
	}
}

