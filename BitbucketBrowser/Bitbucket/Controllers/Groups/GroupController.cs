using CodeBucket.Bitbucket.Controllers;
using CodeBucket.Elements;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;
using CodeBucket.Controllers;

namespace CodeBucket.Bitbucket.Controllers.Groups
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
            SearchPlaceholder = "Search Groups";
		}

        protected override void OnRefresh()
        {
            var sec = new Section();
            if (Model.Count == 0)
                sec.Add(new NoItemsElement("No Groups"));
            else
                Model.ForEach(g => sec.Add(new StyledElement(g.Name, () => NavigationController.PushViewController(new GroupInfoController(Username, g), true))));

            InvokeOnMainThread(delegate {
                Root = new RootElement(Title) { sec };
            });
        }

        protected override List<GroupModel> OnUpdate(bool forced)
        {
            var items = Application.Client.Users[Username].Groups.GetGroups(forced);
            return items.OrderBy(a => a.Name).ToList();
        }
	}
}

