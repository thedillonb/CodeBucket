using MonoTouch.Dialog;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;
using CodeFramework.UI.Controllers;
using CodeFramework.UI.Elements;
using BitbucketBrowser.Controllers.Source;

namespace BitbucketBrowser.Controllers.Branches
{
	public class BranchController : Controller<List<BranchModel>>
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
                Model.ForEach(x => sec.Add(new StyledElement(x.Branch, () => NavigationController.PushViewController(new SourceController(Username, Slug, x.Branch), true))));
           
            InvokeOnMainThread(delegate {
                Root = new RootElement(Title) { sec };
            });
        }

        protected override List<BranchModel> OnUpdate(bool forced)
        {
            var branches = Application.Client.Users[Username].Repositories[Slug].Branches.GetBranches(forced);
            return new List<BranchModel>(branches.Values);
        }
	}
}

