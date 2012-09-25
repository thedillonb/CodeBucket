using System;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;
using CodeFramework.UI.Controllers;
using CodeFramework.UI.Elements;
using BitbucketBrowser.UI.Controllers.Source;

namespace BitbucketBrowser.UI.Controllers.Branches
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
		}
		
        protected override void OnRefresh()
        {
            if (Model.Count == 0)
                return;

            var sec = new Section();
            Model.ForEach(x => {
                var element = new StyledElement(x.Branch);
                element.Tapped += () => NavigationController.PushViewController(new SourceController(Username, Slug, x.Branch), true);
                sec.Add(element);
            });
           
            InvokeOnMainThread(delegate {
                var root = new RootElement(Title) { sec };
                Root = root;
            });
        }

        protected override List<BranchModel> OnUpdate(bool forced)
        {
            var branches = Application.Client.Users[Username].Repositories[Slug].Branches.GetBranches(forced);
            return new List<BranchModel>(branches.Values);
        }
	}
}

