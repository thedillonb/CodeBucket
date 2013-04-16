using MonoTouch.Dialog;
using MonoTouch.UIKit;
using BitbucketSharp.Models;
using System.Linq;
using System.Collections.Generic;
using BitbucketBrowser.Controllers;
using BitbucketBrowser.Elements;


namespace BitbucketBrowser.Controllers.Followers
{
	public abstract class FollowersController : Controller<List<FollowerModel>>
    {
		protected FollowersController()
			: base(true, true)
		{
            Style = UITableViewStyle.Plain;
            Title = "Followers";
            EnableSearch = true;
            AutoHideSearch = true;
            SearchPlaceholder = "Search Followers";
		}

        protected override void OnRefresh()
        {
            Model = Model.OrderBy(a => a.Username).ToList();

            var sec = new Section();
            if (Model.Count == 0)
                sec.Add(new NoItemsElement("No Followers"));
            else
            {
                Model.ForEach(s => {
                    StyledElement sse = new UserElement(s.Username, s.FirstName, s.LastName, s.Avatar);
                    sse.Tapped += () => NavigationController.PushViewController(new ProfileController(s.Username), true);
                    sec.Add(sse);
                });
            }

            InvokeOnMainThread(delegate {
                Root = new RootElement(Title) { sec };
            });
        }
	}
}

