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
    public class GroupController : ModelDrivenController
	{
        public string Username { get; private set; }

		public GroupController(string username) 
            : base(typeof(List<GroupModel>))
		{
            Username = username;
            Title = "Groups";
            SearchPlaceholder = "Search Groups";
            Style = UITableViewStyle.Plain;
		}

        protected override void OnRefresh()
        {
            AddItems<GroupModel>(Model as List<GroupModel>, (group) => {
                return new StyledElement(group.Name, () => NavigationController.PushViewController(new GroupInfoController(Username, group.Slug) { Title = group.Name, Model = group }, true));
            }, "No Groups");
        }

        protected override object OnUpdate(bool forced)
        {
            return Application.Client.Users[Username].Groups.GetGroups(forced).OrderBy(a => a.Name).ToList();
        }
	}
}

