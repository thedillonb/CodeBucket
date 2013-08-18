using CodeBucket.Bitbucket.Controllers;
using BitbucketSharp.Models;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Linq;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CodeBucket.ViewControllers
{
    public class GroupMembersViewController : BaseListControllerDrivenViewController, IListView<UserModel>
    {
        public GroupMembersViewController(string user, string groupName, List<UserModel> model = null)
        {
            EnableSearch = true;
            SearchPlaceholder = "Search Memebers".t();
            NoItemsText = "No Members".t();
            Title = groupName;
            Controller = new GroupMembersController(this, user, groupName) { Model = new ListModel<UserModel> { Data = model } };
        }

        public void Render(ListModel<UserModel> model)
        {
            RenderList(model, s => {
                StyledStringElement sse = new UserElement(s.Username, s.FirstName, s.LastName, s.Avatar);
                sse.Tapped += () => NavigationController.PushViewController(new ProfileViewController(s.Username), true);
                return sse;
            });
        }
    }
}

