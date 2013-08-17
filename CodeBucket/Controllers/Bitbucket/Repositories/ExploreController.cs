using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Threading;
using System.Linq;
using RedPlum;
using System.Drawing;
using MonoTouch;
using CodeBucket.Bitbucket.Controllers.Repositories;
using System.Collections.Generic;
using BitbucketSharp.Models;

namespace CodeBucket.Bitbucket.Controllers
{
    public sealed class ExploreController : RepositoryController
    {
        private string _searchText;

        public ExploreController()
            : base(Application.Account.Username, false, "ExploreController")
        {
            EnableSearch = true;
            AutoHideSearch = false;
            Autorotate = true;
            ShowOwner = true;
            Title = "Explore".t();
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
            readonly ExploreController _container;

            public ExploreSearchDelegate (ExploreController container)
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

        protected override object OnUpdateModel(bool forced)
        {
            if (string.IsNullOrEmpty(_searchText))
                return null;
            return Application.Client.Repositories.Search(_searchText).Repositories;
        }

        public override void SearchButtonClicked(string text)
        {
            _searchText = text;
            View.EndEditing(true);
            UpdateAndRender();
        }
    }
}

