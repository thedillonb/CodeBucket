using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Threading;
using System.Linq;
using RedPlum;
using System.Drawing;
using MonoTouch;
using System.Collections.Generic;
using BitbucketSharp.Models;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using CodeFramework.Filters.Controllers;

namespace CodeBucket.ViewControllers
{
    public sealed class ExploreRepositoriesViewController : BaseListControllerDrivenViewController, IListView<RepositoryDetailedModel>
    {
        public new ExploreRepositoriesController Controller
        {
            get { return (ExploreRepositoriesController)base.Controller; }
            private set { base.Controller = value; }
        }

		public ExploreRepositoriesViewController()
        {
            AutoHideSearch = false;
            EnableFilter = true;
            SearchPlaceholder = "Search Repositories".t();
            NoItemsText = "No Repositories".t();
            Title = "Explore".t();
            Controller = new ExploreRepositoriesController(this);
        }

        public void Render(ListModel<RepositoryDetailedModel> model)
        {
            if (!Controller.Searched)
                return;

            RenderList(model, repo => {
                var description = Application.Account.HideRepositoryDescriptionInList ? string.Empty : repo.Description;
                var sse = new RepositoryElement(repo.Name, repo.FollowersCount, repo.ForkCount, description, repo.Owner, new Uri(repo.LargeLogo(64))) { ShowOwner = true };
                sse.Tapped += () => NavigationController.PushViewController(new RepositoryInfoViewController(repo), true);
                return sse;
            });
        }

        protected override FilterViewController CreateFilterController()
        {
            return new CodeBucket.Filters.ViewControllers.RepositoriesFilterViewController(Controller);
        }

        void ShowSearch(bool value)
        {
            if (!value)
            {
                if (TableView.ContentOffset.Y < 44)
                    TableView.ContentOffset = new PointF (0, 44);
            }
            else
            {
                TableView.ContentOffset = new PointF (0, 0);
            }
        }

        class ExploreSearchDelegate : UISearchBarDelegate 
        {
            readonly ExploreRepositoriesViewController _container;

            public ExploreSearchDelegate (ExploreRepositoriesViewController container)
            {
                _container = container;
            }

            public override void OnEditingStarted (UISearchBar searchBar)
            {
                searchBar.ShowsCancelButton = true;
                _container.SearchStart ();
                _container.ShowSearch(true);
                _container.NavigationController.SetNavigationBarHidden(true, true);
            }

            public override void OnEditingStopped (UISearchBar searchBar)
            {
                searchBar.ShowsCancelButton = false;
                _container.FinishSearch ();
                _container.NavigationController.SetNavigationBarHidden(false, true);
                _container.SearchEnd();
            }

            public override void TextChanged (UISearchBar searchBar, string searchText)
            {
            }

            public override void CancelButtonClicked (UISearchBar searchBar)
            {
                searchBar.ShowsCancelButton = false;
                _container.FinishSearch ();
                searchBar.ResignFirstResponder ();
                _container.NavigationController.SetNavigationBarHidden(false, true);
                _container.SearchEnd();
            }

            public override void SearchButtonClicked (UISearchBar searchBar)
            {
                _container.SearchButtonClicked (searchBar.Text);
                _container.NavigationController.SetNavigationBarHidden(false, true);
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var search = (UISearchBar)TableView.TableHeaderView;
            search.Delegate = new ExploreSearchDelegate(this);
        }

        public override void SearchButtonClicked(string text)
        {
            View.EndEditing(true);
            this.DoWork(() => Controller.Search(text));
        }
    }
}

