using MonoTouch.Dialog;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;
using BitbucketBrowser.Controllers;
using BitbucketBrowser.Elements;
using BitbucketBrowser.Controllers.Source;
using System.Linq;

namespace BitbucketBrowser.Controllers.Branches
{
	public class BranchController : Controller<Dictionary<string, BranchModel>>
	{
        public string Username { get; private set; }
        public string Slug { get; private set; }

		public BranchController(string username, string slug) 
            : base(true, true)
		{
			Style = UITableViewStyle.Plain;
            Username = username;
            Slug = slug;
            Title = "Branches";
            EnableSearch = true;
            AutoHideSearch = true;
            SearchPlaceholder = "Search Branches";
		}
		
        protected override void OnRefresh()
        {
            var sec = new Section();
            if (Model.Count == 0)
                sec.Add(new NoItemsElement("No Branches"));
            else
			{
				foreach (var branchName in Model.Keys.OrderBy(x => x))
				{
					sec.Add(new StyledElement(branchName, () => NavigationController.PushViewController(new SourceController(Username, Slug, branchName), true)));
				}
			}
           
            InvokeOnMainThread(delegate {
                Root = new RootElement(Title) { sec };
            });
        }

		protected override Dictionary<string, BranchModel> OnUpdate(bool forced)
        {
            return Application.Client.Users[Username].Repositories[Slug].Branches.GetBranches(forced);
        }
	}
}

