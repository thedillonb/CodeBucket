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
    public class TeamController : ListController<TeamController.TeamModel>
    {
        public TeamController(IListView<TeamController.TeamModel> view)
            : base(view)
        {
        }

        public override void Update(bool force)
        {
            var model = Application.Client.Account.GetPrivileges(force).Teams.Keys.OrderBy(a => a).Select(x => new TeamModel { Name = x }).ToList();
            model.RemoveAll(x => x.Name.Equals(Application.Account.Username)); //Remove the current user from the 'teams'
            Model = new ListModel<TeamController.TeamModel> { Data = model };
        }

        public class TeamModel
        {
            public string Name { get; set; }
        }
    }
}