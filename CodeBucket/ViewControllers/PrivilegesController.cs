using System;
using System.Linq;
using System.Collections.Generic;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;

namespace CodeBucket.ViewControllers
{
    public class PrivilegesViewController : BaseListControllerDrivenViewController, IListView<PrivilegeModel>
    {
        public event Action<UserModel> SelectedItem;

        protected void OnSelectedItem(UserModel model)
        {
            var handler = SelectedItem;
            if (handler != null)
                handler(model);
        }

        public PrivilegesViewController(string username, string slug, UserModel primary)
        {
            EnableSearch = true;
            Title = "Privileges".t();
            SearchPlaceholder = "Search Users".t();
            NoItemsText = "No Users".t();
            Controller = new PrivilegesController(this, username, slug, primary);
        }
        
        public void Render(ListModel<PrivilegeModel> model)
        {
            RenderList(model, user => {
                StyledStringElement sse = new UserElement(user.User.Username, user.User.FirstName, user.User.LastName, user.User.Avatar);
                sse.Tapped += () => OnSelectedItem(user.User);
                return sse;
            });
        }
    }
}

