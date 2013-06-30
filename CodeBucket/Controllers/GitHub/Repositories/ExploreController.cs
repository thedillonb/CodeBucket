using System;
using CodeBucket.Elements;
using CodeBucket.GitHub.Controllers.Repositories;
using CodeBucket.Views;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Threading;
using System.Linq;
using RedPlum;
using System.Drawing;
using MonoTouch;
using System.Collections.Generic;
using GitHubSharp.Models;

namespace CodeBucket.GitHub.Controllers.Repositories
{
    public sealed class ExploreController : RepositoryController
    {
        public ExploreController()
            : base(Application.Account.Username, true, false)
        {
            EnableSearch = true;
            AutoHideSearch = false;
            Autorotate = true;
            Title = "Explore";
            SearchPlaceholder = "Search Repositories";
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

        protected override List<RepositoryModel> OnUpdate(bool forced)
        {
            return Model;
        }

        public override void SearchButtonClicked(string text)
        {
            View.EndEditing(true);

            var hud = new MBProgressHUD(View.Superview) {Mode = MBProgressHUDMode.Indeterminate, TitleText = "Searching..."};

            InvokeOnMainThread(delegate {
                TableView.TableFooterView.Hidden = true;
                Root.Clear();
                View.Superview.AddSubview(hud);
                hud.Show(true);
            });

            ThreadPool.QueueUserWorkItem(delegate {

                Utilities.PushNetworkActive();

                try
                {
                    var l = Application.GitHubClient.API.SearchRepositories(text);
                    Model = l.Data.Repositories.Select(x => new RepositoryModel { 
                        Description = x.Description,
                        Forks = x.Forks,
                        Name = x.Name,
                        Watchers = x.Watchers,
                        Owner = new BasicUserModel { Login = x.Username }
                    }).ToList();
                    OnRefresh();

                    InvokeOnMainThread(delegate {
                        ShowSearch(Root.Count > 0 && Root[0].Count == 0);
                    });

                }
                catch (Exception e)
                {
                    InvokeOnMainThread(() => Utilities.ShowAlert("Error to Load", e.Message));
                }

                Utilities.PopNetworkActive();

                if (hud != null)
                {
                    InvokeOnMainThread(delegate {
                        hud.Hide(true);
                        hud.RemoveFromSuperview();
                    });
                }
            });
        }
    }
}


