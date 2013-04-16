using System;
using BitbucketBrowser.Data;
using BitbucketBrowser.GitHub.Controllers.Repositories;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Threading;
using System.Linq;
using RedPlum;
using System.Drawing;
using MonoTouch;
using CodeFramework.UI.Views;
using BitbucketBrowser.Elements;

namespace BitbucketBrowser.GitHub.Controllers
{
    public sealed class ExploreController : DialogViewController
    {

        public ExploreController()
            : base(UITableViewStyle.Plain, new RootElement("Explore"), false)
        {
            EnableSearch = true;
            AutoHideSearch = false;
            NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);
            Autorotate = true;
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
                _container.StartSearch ();
                _container.ShowSearch(true);
                _container.NavigationController.SetNavigationBarHidden(true, true);
            }

            public override void OnEditingStopped (UISearchBar searchBar)
            {
                searchBar.ShowsCancelButton = false;
                _container.FinishSearch ();
                _container.NavigationController.SetNavigationBarHidden(false, true);
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

            TableView.BackgroundColor = UIColor.Clear;
            WatermarkView.AssureWatermark(this);

            TableView.TableFooterView = new DropbarView(View.Bounds.Width) {Hidden = true};
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
                    var l = Application.GitHubClient.API.SearchRepositories(text).Data;
                    var sec = new Section();

                    foreach (var repo in l.Repositories.OrderByDescending(x => x.Watchers))
                    {
                        var r = repo;
                        var el = new RepositoryElement(r);
                        el.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(r.Owner, r.Name), true);
                        sec.Add(el);
                    }


                    InvokeOnMainThread(delegate {
                        TableView.TableFooterView.Hidden = sec.Elements.Count == 0;
                        Root = new RootElement(Title) { sec };
                        hud.Hide(true);
                        hud.RemoveFromSuperview();

                        ShowSearch(sec.Count == 0);
                    });

                }
                catch (Exception e)
                {
                    InvokeOnMainThread(() => Utilities.ShowAlert("Error to Load", e.Message));
                }

                Utilities.PopNetworkActive();

                InvokeOnMainThread(delegate {
                    hud.Hide(true);
                    hud.RemoveFromSuperview();
                });
            });
        }
    }
}


