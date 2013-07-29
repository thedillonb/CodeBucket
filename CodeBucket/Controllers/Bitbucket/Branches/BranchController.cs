using MonoTouch.Dialog;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;
using CodeBucket.Bitbucket.Controllers;
using CodeBucket.Bitbucket.Controllers.Source;
using System.Linq;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using System.Threading.Tasks;

namespace CodeBucket.Bitbucket.Controllers.Branches
{
    public class BranchController : ModelDrivenController
	{
        public string Username { get; private set; }
        public string Slug { get; private set; }

		public BranchController(string username, string slug) 
            : base(typeof(List<BranchModel>))
		{
            Username = username;
            Slug = slug;
            Title = "Branches";
            SearchPlaceholder = "Search Branches";
            Style = UITableViewStyle.Plain;
		}

        protected override void OnRefresh()
        {
            AddItems<BranchModel>(Model as List<BranchModel>, 
                                  (o) => new StyledElement(o.Branch, () => NavigationController.PushViewController(new SourceController(Username, Slug, o.Branch), true)),
                                  "No Branches");
        }

        protected override object OnUpdate(bool forced)
        {
            return Application.Client.Users[Username].Repositories[Slug].Branches.GetBranches(forced).Values.OrderBy(x => x.Branch).ToList();
        }

	}
}

