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
    public class BranchController : BaseController
	{
        public List<BranchModel> Model { get; set; }
        public string Username { get; private set; }
        public string Slug { get; private set; }

		public BranchController(string username, string slug) 
            : base(true)
		{
            Username = username;
            Slug = slug;
            Title = "Branches";
            SearchPlaceholder = "Search Branches";
            Style = UITableViewStyle.Plain;
		}

        protected override async Task DoRefresh(bool force)
        {
            if (Model == null || force)
                await Task.Run(() => { Model = Application.Client.Users[Username].Repositories[Slug].Branches.GetBranches(force).Values.OrderBy(x => x.Branch).ToList(); });
            AddItems<BranchModel>(Model, (o) => new StyledElement(o.Branch, () => NavigationController.PushViewController(new SourceController(Username, Slug, o.Branch), true)));
        }
	}
}

