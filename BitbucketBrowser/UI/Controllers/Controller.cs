using System;
using MonoTouch.Dialog;
using System.Threading;
using MonoTouch.UIKit;
using RedPlum;
using System.Linq;
using System.Drawing;

namespace BitbucketBrowser.UI
{
    public abstract class Controller<T> : DialogViewController
    {
        public T Model { get; set; }

        private bool _loaded = false;

        public bool Loaded { get { return _loaded; } }

        private bool isSearching = false;


        class CustomSearchDelegate : UISearchBarDelegate 
        {
            Controller<T> container;

            public CustomSearchDelegate (Controller<T> container)
            {
                this.container = container;
            }

            public override void OnEditingStarted (UISearchBar searchBar)
            {
                searchBar.ShowsCancelButton = true;
                container.StartSearch ();
                container.NavigationController.SetNavigationBarHidden(true, true);
                container.isSearching = true;
            }

            public override void OnEditingStopped (UISearchBar searchBar)
            {
                //searchBar.ShowsCancelButton = true;
            }

            public override void TextChanged (UISearchBar searchBar, string searchText)
            {
                container.PerformFilter(searchText);
            }

            public override void CancelButtonClicked (UISearchBar searchBar)
            {
                searchBar.Text = "";
                searchBar.ShowsCancelButton = false;
                container.FinishSearch ();
                searchBar.ResignFirstResponder ();
                container.NavigationController.SetNavigationBarHidden(false, true);
                container.isSearching = false;
            }

            public override void SearchButtonClicked (UISearchBar searchBar)
            {
                //container.SearchButtonClicked (searchBar.Text);
                searchBar.ResignFirstResponder();

                
                //Enable the cancel button again....
                foreach (var s in searchBar.Subviews)
                {
                    var x = s as UIButton;
                    if (x != null)
                        x.Enabled = true;
                }
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (NavigationController != null && isSearching)
                NavigationController.SetNavigationBarHidden(true, true);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (NavigationController != null && NavigationController.NavigationBarHidden)
                NavigationController.SetNavigationBarHidden(false, true);
        }


        public Controller(bool push = false, bool refresh = false)
            : base(new RootElement(""), push)
        {
            if (refresh)
                RefreshRequested += (sender, e) => Refresh(true);

            var button = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null); 
            NavigationItem.BackBarButtonItem = button;
            SearchPlaceholder = "Search";

            Autorotate = true;
        }

        class RefreshView : RefreshTableHeaderView
        {
            public RefreshView(RectangleF rect)
                : base(rect)
            {
                BackgroundColor = UIColor.Clear;
                StatusLabel.BackgroundColor = UIColor.Clear;
                LastUpdateLabel.BackgroundColor = UIColor.Clear;
            }

            public override void Draw(RectangleF rect)
            {
                base.Draw(rect);
                //Stop the super class from doing stupid shit like drawing the shadow
            }
        }


        public override RefreshTableHeaderView MakeRefreshTableHeaderView(RectangleF rect)
        {
            return new RefreshView(rect);
        }

            
        protected abstract void OnRefresh();

        protected abstract T OnUpdate();


        public override void ViewDidLoad()
        {
            var search = this.TableView.TableHeaderView as UISearchBar;
            if (search != null)
                search.Delegate = new CustomSearchDelegate(this);

            Root.Caption = this.Title;
            TableView.BackgroundColor = UIColor.Clear;
            if (Style != UITableViewStyle.Grouped)
            {
                TableView.TableFooterView = new DropbarElement(View.Bounds.Width);
                TableView.TableFooterView.Hidden = true;
            }

            WatermarkView.AssureWatermark(this);
            base.ViewDidLoad();
        }


        public void Refresh(bool force = false)
        {
            if (Model != null && !force)
            {
                try
                {
                    OnRefresh();
                }
                catch (Exception e)
                {
                    InvokeOnMainThread(() => ErrorView.Show(this.View.Superview, e.Message));
                }

                InvokeOnMainThread(delegate { 
                    if (TableView.TableFooterView != null)
                        TableView.TableFooterView.Hidden = this.Root.Count == 0;

                    ReloadComplete(); 
                });
                _loaded = true;
                return;
            }

            MBProgressHUD hud = null;
            if (!force) {
                hud = new MBProgressHUD(this.View.Superview); 
                hud.Mode = MBProgressHUDMode.Indeterminate;
                hud.TitleText = "Loading...";
                this.View.Superview.AddSubview(hud);
                hud.Show(true);
            }

            ThreadPool.QueueUserWorkItem(delegate {
                try
                {
                    Model = OnUpdate();
                    Refresh();
                }
                catch (Exception e)
                {
                    InvokeOnMainThread(() => ErrorView.Show(this.View.Superview, e.Message));
                }


                if (hud != null)
                {
                    InvokeOnMainThread(delegate {
                        hud.Hide(true);
                        hud.RemoveFromSuperview();
                    });
                }
            });
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (!_loaded)
            {
                Refresh();
                _loaded = true;
            }
        }
    }
}

