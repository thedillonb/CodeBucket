using CodeBucket.Bitbucket.Controllers;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using System.Threading.Tasks;

namespace CodeBucket.Bitbucket.Controllers.Groups
{
    public class GroupController : BaseController
	{
        public List<GroupModel> Model { get; set; }

        public string Username { get; private set; }

		public GroupController(string username, bool push = true) 
            : base(push)
		{
            Username = username;
            Title = "Groups";
            SearchPlaceholder = "Search Groups";
            Style = UITableViewStyle.Plain;
		}

        protected override async Task DoRefresh(bool force)
        {
            if (Model == null || force)
                await Task.Run(() => { Model = Application.Client.Users[Username].Groups.GetGroups(force).OrderBy(a => a.Name).ToList(); });
            AddItems<GroupModel>(Model, (group) => {
                return new StyledElement(group.Name, () => NavigationController.PushViewController(new GroupInfoController(Username, group.Slug) { Title = group.Name, Model = group }, true));
            });
        }
	}
}

