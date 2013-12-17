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

namespace CodeBucket.ViewControllers
{
    public abstract class FollowersViewController : BaseListControllerDrivenViewController, IListView<FollowerModel>
    {
        protected FollowersViewController()
		{
            Title = "Followers".t();
            SearchPlaceholder = "Search Followers".t();
            NoItemsText = "No Followers".t();
		}

        public void Render(ListModel<FollowerModel> model)
        {
            RenderList(model, s => {
                StyledStringElement sse = new UserElement(s.Username, s.FirstName, s.LastName, s.Avatar);
                sse.Tapped += () => NavigationController.PushViewController(new ProfileViewController(s.Username), true);
                return sse;
            });
        }
	}

    public class UserFollowersViewController : FollowersViewController
    {
        public UserFollowersViewController(string username)
        {
            Controller = new UserFollowersController(this, username);
        }
    }

    public class RepoFollowersViewController : FollowersViewController
    {
        public RepoFollowersViewController(string username, string slug)
        {
            Controller = new RepoFollowersController(this, username, slug);
        }
    }
}

