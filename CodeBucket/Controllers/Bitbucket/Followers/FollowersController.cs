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
using CodeBucket.Views.Accounts;

namespace CodeBucket.Bitbucket.Controllers.Followers
{
    public abstract class FollowersController : BaseListModelController
    {
		protected FollowersController()
		{
            Title = "Followers".t();
            SearchPlaceholder = "Search Followers".t();
            NoItemsText = "No Followers".t();
            Style = UITableViewStyle.Plain;
		}

        protected override Element CreateElement(object obj)
        {
            var s = (FollowerModel)obj;
            StyledStringElement sse = new UserElement(s.Username, s.FirstName, s.LastName, s.Avatar);
            sse.Tapped += () => NavigationController.PushViewController(new ProfileView(s.Username), true);
            return sse;
        }
	}
}

