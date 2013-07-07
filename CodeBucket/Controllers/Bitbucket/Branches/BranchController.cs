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

namespace CodeBucket.Bitbucket.Controllers.Branches
{
    public class BranchController : ListController<BranchModel>
	{
        public string Username { get; private set; }
        public string Slug { get; private set; }

		public BranchController(string username, string slug) 
            : base(true)
		{
            Username = username;
            Slug = slug;
            Title = "Branches";
            SearchPlaceholder = "Search Branches";
		}

        protected override List<BranchModel> GetData(bool force, int currentPage, out int nextPage)
        {
            var d = Application.Client.Users[Username].Repositories[Slug].Branches.GetBranches(force);
            nextPage = -1;
            return d.Values.OrderBy(x => x.Branch).ToList();
        }

        protected override Element CreateElement(BranchModel obj)
        {
            return new StyledElement(obj.Branch, () => NavigationController.PushViewController(new SourceController(Username, Slug, obj.Branch), true));
        }
	}
}

