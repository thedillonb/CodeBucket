using MonoTouch.Dialog;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;
using CodeFramework.UI.Controllers;
using CodeFramework.UI.Elements;
using BitbucketBrowser.Controllers.Source;

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
				foreach (var entry in Model)
				{
					var branch = entry;
					sec.Add(new StyledElement(branch.Key, () => NavigationController.PushViewController(new SourceController(Username, Slug, branch.Key), true)));
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

