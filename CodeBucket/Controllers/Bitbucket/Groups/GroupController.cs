using CodeBucket.Bitbucket.Controllers;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;

namespace CodeBucket.Bitbucket.Controllers.Groups
{
    public class GroupController : ListController
	{
        public string Username { get; private set; }

		public GroupController(string username, bool push = true) 
            : base(push)
		{
            Username = username;
            Title = "Groups";
            SearchPlaceholder = "Search Groups";
		}

        protected override object GetData(bool force, int currentPage, out int nextPage)
        {
            var items = Application.Client.Users[Username].Groups.GetGroups(force);
            nextPage = -1;
            return items.OrderBy(a => a.Name).ToList();
        }

        protected override Element CreateElement(object obj)
        {
            var group = obj as GroupModel;
            return new StyledElement(group.Name, () => NavigationController.PushViewController(new GroupInfoController(Username, group.Slug) { Title = group.Name, Model = group }, true));
        }
	}
}

