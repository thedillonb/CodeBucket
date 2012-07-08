using System;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;


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
		}
		
        protected override void OnRefresh()
        {
            var r = new RootElement(Title);
            var sec = new Section();
            Model.ForEach(g =>
            {
                var el = new StyledElement(g.Name, () => NavigationController.PushViewController(new GroupInfoController(Username, g), true))
                { Accessory = UITableViewCellAccessory.DisclosureIndicator };
                sec.Add(el);
            });
            r.Add(sec);

            InvokeOnMainThread(delegate {
                Root = r;
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
        }

        protected override void OnRefresh ()
        {
            var root = new RootElement(Title);
            var sec = new Section();
            Model.Members.OrderBy(x => x.Username).ToList().ForEach(x =>
            {
                var realName = x.FirstName ?? "" + " " + x.LastName ?? "";
                StyledElement sse;
                if (!string.IsNullOrWhiteSpace(realName))
                    sse = new StyledElement(x.Username, realName, UITableViewCellStyle.Subtitle);
                else
                    sse = new StyledElement(x.Username);
                sse.Tapped += () => NavigationController.PushViewController(new ProfileController(x.Username), true);
                sse.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                sec.Add(sse);
            });
            root.UnevenRows = true;
            root.Add(sec);


            InvokeOnMainThread(delegate {
                    Root = root;
            });
        }

        protected override GroupModel OnUpdate ()
        {
            return Application.Client.Users[User].Groups[Model.Slug].GetInfo();
        }
    }
}

