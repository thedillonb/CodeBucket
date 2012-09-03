using System;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;
using CodeFramework.UI.Controllers;
using CodeFramework.UI.Elements;


namespace BitbucketBrowser.UI
{
	public class GroupController : Controller<List<GroupModel>>
	{
        public string Username { get; private set; }

		public GroupController(string username, bool push = true) 
            : base(push, true)
		{
			Style = UITableViewStyle.Plain;
            Username = username;
            Title = "Groups";
            EnableSearch = true;
            AutoHideSearch = true;
		}
		
        protected override void OnRefresh()
        {
            if (Model.Count == 0)
                return;

            var sec = new Section();
            Model.ForEach(g =>
            {
                var el = new StyledElement(g.Name, () => NavigationController.PushViewController(new GroupInfoController(Username, g), true))
                { Accessory = UITableViewCellAccessory.DisclosureIndicator };
                sec.Add(el);
            });

            InvokeOnMainThread(delegate {
                Root = new RootElement(Title) { sec }; 
            });
        }

        protected override List<GroupModel> OnUpdate()
        {
            var g = Application.Client.Users[Username].Groups.GetGroups();
            return g.OrderBy(x => x.Name).ToList();
        }
	}

    public class GroupInfoController : Controller<GroupModel>
    {
        public string User { get; private set; }

        public GroupInfoController(string user, GroupModel group)
            : base(true, true)
        {
            Style = UITableViewStyle.Plain;
            User = user;
            Model = group;
            Title = group.Name;
            EnableSearch = true;
            AutoHideSearch = true;
        }

        protected override void OnRefresh ()
        {
            var sec = new Section();
            Model.Members.OrderBy(x => x.Username).ToList().ForEach(s => {
                StyledElement sse = new UserElement(s.Username, s.FirstName, s.LastName, s.Avatar);
                sse.Tapped += () => NavigationController.PushViewController(new ProfileController(s.Username), true);
                sec.Add(sse);
            });

            InvokeOnMainThread(delegate {
                Root = new RootElement(Title) { sec };
            });
        }

        protected override GroupModel OnUpdate ()
        {
            return Application.Client.Users[User].Groups[Model.Slug].GetInfo();
        }
    }
}

