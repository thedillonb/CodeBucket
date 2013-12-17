using MonoTouch.Dialog;
using MonoTouch.UIKit;
using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;
using CodeBucket.Bitbucket.Controllers;
using System;
using System.Drawing;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using CodeFramework.Views;
using CodeFramework.Filters.Controllers;
using CodeBucket.Filters.Models;
using CodeBucket.Controllers;


namespace CodeBucket.ViewControllers
{
    public class RepositoriesViewController : BaseListControllerDrivenViewController, IListView<RepositoryDetailedModel>
    {
        public string Username { get; private set; }
        public bool ShowOwner { get; set; }

		public new RepositoriesController Controller
		{
			get { return (RepositoriesController)base.Controller; }
			protected set { base.Controller = value; }
		}
    
        public RepositoriesViewController(string username, bool refresh = true)
            : base(refresh: refresh)
        {
            Username = username;
            ShowOwner = false;
            EnableFilter = true;
            Title = "Repositories".t();
            SearchPlaceholder = "Search Repositories".t();
            NoItemsText = "No Repositories".t();

            Controller = new RepositoriesController(this, username);
        }

        public void Render(ListModel<RepositoryDetailedModel> model)
        {
            RenderList(model, repo => {
                var description = Application.Account.HideRepositoryDescriptionInList ? string.Empty : repo.Description;
                var sse = new RepositoryElement(repo.Name, repo.FollowersCount, repo.ForkCount, description, repo.Owner, new Uri(repo.LargeLogo(64))) { ShowOwner = ShowOwner };
                sse.Tapped += () => NavigationController.PushViewController(new RepositoryInfoViewController(repo), true);
                return sse;
            });
        }

        protected override FilterViewController CreateFilterController()
        {
            return new CodeBucket.Filters.ViewControllers.RepositoriesFilterViewController(Controller);
        }
    }
}