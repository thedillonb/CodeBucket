using MonoTouch.Dialog;
using MonoTouch.UIKit;
using GitHubSharp.Models;
using System.Linq;
using System.Collections.Generic;
using BitbucketBrowser.Controllers;
using BitbucketBrowser.Elements;


namespace BitbucketBrowser.GitHub.Controllers.Followers
{
	public abstract class FollowersController : ListController<BasicUserModel>
    {
		protected FollowersController()
			: base(true)
		{
            Title = "Followers";
		}

        protected override Element CreateElement(BasicUserModel s)
        {
            StyledElement sse = new UserElement(s.Login, null, null, s.AvatarUrl);
            sse.Tapped += () => NavigationController.PushViewController(new ProfileController(s.Login), true);
            return sse;
        }
	}
}
