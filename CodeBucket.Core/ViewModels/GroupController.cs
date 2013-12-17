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

namespace CodeBucket.Controllers
{
    public class GroupController : ListController<GroupModel>
	{
        public string Username { get; private set; }

		public GroupController(IView<ListModel<GroupModel>> view, string username) 
            : base(view)
		{
            Username = username;
		}

        public override void Update(bool force)
        {
            var data = Application.Client.Users[Username].Groups.GetGroups(force).OrderBy(a => a.Name).ToList();
            Model = new ListModel<GroupModel> { Data = data };
        }
	}
}

