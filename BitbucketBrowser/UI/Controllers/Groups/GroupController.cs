using System;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;
using CodeFramework.UI.Controllers;
using CodeFramework.UI.Elements;

namespace BitbucketBrowser.UI.Controllers.Groups
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
                var root = new RootElement(Title) { sec };
                Root = root;
            });
        }

        protected override List<GroupModel> OnUpdate(bool forced)
        {
            var g = Application.Client.Users[Username].Groups.GetGroups(forced);
            return g.OrderBy(x => x.Name).ToList();
        }
	}
}

