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
using CodeBucket.Views.Accounts;

namespace CodeBucket.Bitbucket.Controllers.Groups
{
    public class GroupMembersController : BaseListModelController
    {
        public new List<UserModel> Model
        {
            get { return (List<UserModel>)base.Model; }
            set { base.Model = value; }
        }

        public string User { get; private set; }

        public string GroupName { get; private set; }
        
        public GroupMembersController(string user, string groupName)
        {
            Style = UITableViewStyle.Plain;
            User = user;
            EnableSearch = true;
            SearchPlaceholder = "Search Memebers".t();
            NoItemsText = "No Members".t();
            Title = groupName;
            GroupName = groupName;
        }

        protected override object OnUpdateListModel(bool forced, int currentPage, ref int nextPage)
        {
            return Application.Client.Users[User].Groups[GroupName].GetInfo(forced).Members;
        }

        protected override Element CreateElement(object obj)
        {
            var s = (UserModel)obj;
            StyledStringElement sse = new UserElement(s.Username, s.FirstName, s.LastName, s.Avatar);
            sse.Tapped += () => NavigationController.PushViewController(new ProfileView(s.Username), true);
            return sse;
        }
    }
}

