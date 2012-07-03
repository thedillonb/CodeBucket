using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using BitbucketSharp.Models;
using System.Linq;
using System.Threading;
using BitbucketSharp;
using System.Collections.Generic;


namespace BitbucketBrowser.UI
{
	public abstract class FollowersController : Controller<List<FollowerModel>>
    {
		protected FollowersController()
			: base(true, true)
		{
            Style = UITableViewStyle.Plain;
            Title = "Followers";
		}

        protected override void OnRefresh()
        {
            BeginInvokeOnMainThread(delegate {
                Root.Clear();
                var sec = new Section();
                Model.ForEach(s =>
                {
                    var realName = s.FirstName ?? "" + " " + s.LastName ?? "";
                    SubcaptionElement sse;
                    if (!string.IsNullOrWhiteSpace(realName))
                        sse = new SubcaptionElement(s.Username, realName);
                    else
                        sse = new SubcaptionElement(s.Username);
                    sse.Tapped += () => NavigationController.PushViewController(new ProfileController(s.Username), true);
                    sse.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                    sec.Add(sse);
                });
                Root.Add(sec);
            });
        }
	}
	
	public class UserFollowersController : FollowersController
	{
		private readonly string _name;

		public UserFollowersController(string name)
		{
			_name = name;
		}

        protected override List<FollowerModel> OnUpdate()
        {
            var f = Application.Client.Users[_name].GetFollowers().Followers;
            return f.OrderBy(x => x.Username).ToList();
        }
	}
	
	public class RepoFollowersController : FollowersController
	{
		private readonly string _name;
		private readonly string _owner;

		public RepoFollowersController(string owner, string name)
		{
			_name = name;
			_owner = owner;
		}

        protected override List<FollowerModel> OnUpdate()
        {
            var f = Application.Client.Users[_owner].Repositories[_name].GetFollowers().Followers;
            return f.OrderBy(x => x.Username).ToList();
        }
	}
}

