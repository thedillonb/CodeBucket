using MonoTouch.Dialog;
using GitHubSharp.Models;
using BitbucketBrowser.Controllers;
using BitbucketBrowser.Elements;
using ProfileController = CodeBucket.GitHub.Controllers.Accounts.ProfileController;

namespace CodeBucket.GitHub.Controllers.Followers
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
