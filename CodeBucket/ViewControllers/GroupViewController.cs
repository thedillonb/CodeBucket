using CodeFramework.Controllers;
using System.Collections.Generic;
using BitbucketSharp.Models;
using MonoTouch.UIKit;
using CodeBucket.Controllers;
using MonoTouch.Dialog;

namespace CodeBucket.ViewControllers
{
    public class GroupViewController : BaseListControllerDrivenViewController, IListView<GroupModel>
	{
        private readonly string _username;

		public GroupViewController(string username) 
		{
            _username = username;
            Style = UITableViewStyle.Plain;
            Title = "Groups".t();
            SearchPlaceholder = "Search Groups".t();
            NoItemsText = "No Groups".t();
            Controller = new GroupController(this, username);
		}

        public void Render(ListModel<GroupModel> model)
        {
            RenderList(model, x => {
                return new StyledStringElement(x.Name, () => NavigationController.PushViewController(new GroupMembersViewController(_username, x.Slug, x.Members) { Title = x.Name }, true));
            });
        }
	}
}

