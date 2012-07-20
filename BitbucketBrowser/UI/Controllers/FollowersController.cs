using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using BitbucketSharp.Models;
using System.Linq;
using System.Threading;
using BitbucketSharp;
using System.Collections.Generic;
using MonoTouch.Dialog.Utilities;


namespace BitbucketBrowser.UI
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
		}

        protected override void OnRefresh()
        {
            if (Model.Count == 0)
                return;

            var sec = new Section();
            Model.ForEach(s => {
                StyledElement sse = new UserElement(s);
                sse.Tapped += () => NavigationController.PushViewController(new ProfileController(s.Username), true);
                sec.Add(sse);
            });

            InvokeOnMainThread(delegate {
                Root = new RootElement(Title) { sec };
            });
        }


        private class UserElement : SubcaptionElement, IImageUpdated
        {
            private UITableViewCell _cell;
            private FollowerModel _model;

            public UserElement(FollowerModel s)
                : base (s.Username)
            {
                _model = s;
                var realName = s.FirstName ?? "" + " " + s.LastName ?? "";
                 if (!string.IsNullOrWhiteSpace(realName))
                    Value = realName;
                Accessory = UITableViewCellAccessory.DisclosureIndicator;
                ImageUri = new Uri(s.Avatar);
            }
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

