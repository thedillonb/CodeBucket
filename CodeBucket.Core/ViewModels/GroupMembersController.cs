using CodeFramework.Controllers;
using BitbucketSharp.Models;
using System.Collections.Generic;

namespace CodeBucket.Controllers
{
    public class GroupMembersController : ListController<UserModel>
    {
        public string User { get; private set; }

        public string GroupName { get; private set; }
        
        public GroupMembersController(IView<ListModel<UserModel>> view, string user, string groupName)
            : base(view)
        {
            User = user;
            GroupName = groupName;
        }

        public override void Update(bool force)
        {
            Model = new ListModel<UserModel> {
                Data = Application.Client.Users[User].Groups[GroupName].GetInfo(force).Members
            };
        }
    }
}

