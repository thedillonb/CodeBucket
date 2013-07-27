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
    public class BranchController : ListController
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

        protected override object GetData(bool force, int currentPage, out int nextPage)
        {
            var d = Application.Client.Users[Username].Repositories[Slug].Branches.GetBranches(force);
            nextPage = -1;
            return d.Values.OrderBy(x => x.Branch).ToList();
        }

        protected override Element CreateElement(object obj)
        {
            var o = obj as BranchModel;
            return new StyledElement(o.Branch, () => NavigationController.PushViewController(new SourceController(Username, Slug, o.Branch), true));
        }
	}
}

