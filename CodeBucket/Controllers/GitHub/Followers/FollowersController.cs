using MonoTouch.Dialog;
using GitHubSharp.Models;
using ProfileController = CodeBucket.GitHub.Controllers.Accounts.ProfileController;
using CodeBucket.Controllers;
using CodeBucket.Elements;

namespace CodeBucket.GitHub.Controllers.Followers
{
	public abstract class FollowersController : ListController<BasicUserModel>
    {
		protected FollowersController()
			: base(true)
		{
            Title = "Followers";
            SearchPlaceholder = "Search Followers";
		}

        protected override Element CreateElement(BasicUserModel s)
        {
            StyledElement sse = new UserElement(s.Login, null, null, s.AvatarUrl);
            sse.Tapped += () => NavigationController.PushViewController(new ProfileController(s.Login), true);
            return sse;
        }
	}
}
