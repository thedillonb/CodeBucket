using CodeBucket.GitHub.Controllers.Source;
using GitHubSharp.Models;
using MonoTouch.Dialog;
using System.Collections.Generic;
using System.Linq;
using CodeBucket.Controllers;
using CodeBucket.Elements;

namespace CodeBucket.GitHub.Controllers.Branches
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
            var d = Application.GitHubClient.API.GetBranches(Username, Slug);
            nextPage = -1;
            return d.Data.OrderByDescending(x => x.Name).ToList();
        }

        protected override Element CreateElement(BranchModel obj)
        {
            return new StyledElement(obj.Name, () => NavigationController.PushViewController(new SourceController(Username, Slug, obj.Name), true));
        }
	}
}


