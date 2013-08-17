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
using CodeBucket.Views.Accounts;

namespace CodeBucket.Bitbucket.Controllers.Teams
{
    public class TeamController : BaseListModelController
    {
        public TeamController() 
        {
            Title = "Teams".t();
            SearchPlaceholder = "Search Teams".t();
            NoItemsText = "No Teams".t();
        }

        protected override object OnUpdateListModel(bool forced, int currentPage, ref int nextPage)
        {
            var model = Application.Client.Account.GetPrivileges(forced).Teams.Keys.OrderBy(a => a).ToList();
            model.Remove(Application.Account.Username); //Remove the current user from the 'teams'
            return model;
        }

        protected override Element CreateElement(object obj)
        {
            var o = obj.ToString();
            return new StyledStringElement(o, () => NavigationController.PushViewController(new ProfileView(o), true));
        }
    }
}