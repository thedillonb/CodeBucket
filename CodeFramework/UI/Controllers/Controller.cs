using System;
using MonoTouch.Dialog;
using System.Threading;
using MonoTouch.UIKit;
using RedPlum;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch;
using CodeFramework.UI.Views;
using CodeFramework.UI.Elements;

namespace CodeFramework.UI.Controllers
{
    public abstract class Controller<T> : DialogViewController
    {
        public T Model { get; set; }

        private bool _loaded = false;

        public bool Loaded { get { return _loaded; } }

        protected bool isSearching = false;

        private ErrorView _currentError;

        class CustomSearchDelegate : UISearchBarDelegate 
        {
            Controller<T> container;
            DialogViewController searchController;
            List<ElementContainer> searchElements;

            static UIColor NoItemColor = UIColor.FromRGBA(0.1f, 0.1f, 0.1f, 0.9f);

            class ElementContainer
            {
                public Element Element;
                public Element Parent;
            }

            public CustomSearchDelegate (Controller<T> container)
            {
                this.container = container;
            }

            public override void OnEditingStarted (UISearchBar searchBar)
            {
                container.SearchStart();

                if (searchController == null)
                {
                    searchController = new DialogViewController(UITableViewStyle.Plain, null);
                    searchController.LoadView();
                    searchController.TableView.TableFooterView = new DropbarView(1f);
                }

                searchBar.ShowsCancelButton = true;
                container.StartSearch ();
                container.TableView.ScrollRectToVisible(new RectangleF(0, 0, 1, 1), false);
                container.NavigationController.SetNavigationBarHidden(true, true);
                container.isSearching = true;
                container.TableView.ScrollEnabled = false;

                if (searchController.Root != null && searchController.Root.Count > 0 && searchController.Root[0].Count > 0)
                {
                    searchController.TableView.TableFooterView.Hidden = false;
                    searchController.View.BackgroundColor = UIColor.White;
                    searchController.TableView.ScrollEnabled = true;
                }
                else
                {
                    searchController.TableView.TableFooterView.Hidden = true;
                    searchController.View.BackgroundColor = NoItemColor;
                    searchController.TableView.ScrollEnabled = false;
                }

                searchElements = new List<ElementContainer>();

                //Grab all the elements that we could search trhough
                foreach (var s in container.Root)
                    foreach (var e in s.Elements)
                        searchElements.Add(new ElementContainer() { Element = e, Parent = e.Parent });

                if (!container.ChildViewControllers.Contains(searchController))
                {
                    System.Diagnostics.Debug.WriteLine("Editing started: " + new RectangleF(container.TableView.Bounds.X, 44f, container.TableView.Bounds.Width, container.TableView.Bounds.Height - 44f));
                    searchController.View.Frame = new RectangleF(container.TableView.Bounds.X, 44f, container.TableView.Bounds.Width, container.TableView.Bounds.Height - 44f);
                    container.AddChildViewController(searchController);
                    container.View.AddSubview(searchController.View);
                }


            }

            public override void OnEditingStopped (UISearchBar searchBar)
            {

            }

            public override void TextChanged (UISearchBar searchBar, string searchText)
            {
                if (string.IsNullOrEmpty(searchText))
                {
                    if (searchController.Root != null)
                        searchController.Root.Clear();
                    searchController.View.BackgroundColor = NoItemColor;
                    searchController.TableView.TableFooterView.Hidden = true;
                    searchController.TableView.ScrollEnabled = false;
                    return;
                }
                    
                var sec = new Section();
                foreach (var el in searchElements)
                {
                    if (el.Element.Matches(searchText))
                    {
                        sec.Add(el.Element);
                    }
                }
                searchController.TableView.ScrollEnabled = true;

                if (sec.Count == 0)
                {
                    sec.Add(new NoItemsElement());
                }

                searchController.View.BackgroundColor = UIColor.White;
                searchController.TableView.TableFooterView.Hidden = sec.Count == 0;
                var root = new RootElement("") { sec };
                root.UnevenRows = true;
                searchController.Root = root;
            }

            public override void CancelButtonClicked (UISearchBar searchBar)
            {
                //Reset the parent
                foreach (var s in searchElements)
                    s.Element.Parent = s.Parent;


                searchBar.Text = "";
                searchBar.ShowsCancelButton = false;
                container.FinishSearch ();
                searchBar.ResignFirstResponder ();
                container.NavigationController.SetNavigationBarHidden(false, true);
                container.isSearching = false;
                container.TableView.ScrollEnabled = true;

                searchController.RemoveFromParentViewController();
                searchController.View.RemoveFromSuperview();

                if (searchController.Root != null)
                    searchController.Root.Clear();

                searchElements.Clear();
                searchElements = null;

                container.SearchEnd();
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

        public virtual void SearchStart()
        {
        }

        public virtual void SearchEnd()
        {
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (NavigationController != null && isSearching)
                NavigationController.SetNavigationBarHidden(true, true);
            if (isSearching)
            {
                TableView.ScrollRectToVisible(new RectangleF(0, 0, 1, 1), false);
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (NavigationController != null && NavigationController.NavigationBarHidden)
                NavigationController.SetNavigationBarHidden(false, true);

            if (isSearching)
            {
                View.EndEditing(true);
                var searchBar = TableView.TableHeaderView as UISearchBar;
                if (searchBar != null)
                {
                    //Enable the cancel button again....
                    foreach (var s in searchBar.Subviews)
                    {
                        var x = s as UIButton;
                        if (x != null)
                            x.Enabled = true;
                    }
                }
            }
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
                TableView.TableFooterView = new DropbarView(View.Bounds.Width);
                TableView.TableFooterView.Hidden = true;
            }

            WatermarkView.AssureWatermark(this);
            base.ViewDidLoad();
        }


        public void Refresh(bool force = false)
        {
            InvokeOnMainThread(delegate {
                if (_currentError != null)
                    _currentError.RemoveFromSuperview();
                _currentError = null;
            });

            if (Model != null && !force)
            {
                try
                {
                    OnRefresh();
                }
                catch (Exception e)
                {
                    InvokeOnMainThread(() => _currentError = ErrorView.Show(this.View.Superview, e.Message));
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
                    Utilities.PushNetworkActive();
                    Model = OnUpdate();
                    if (Model != null)
                        Refresh();
                }
                catch (Exception e)
                {
                    InvokeOnMainThread(() => _currentError = ErrorView.Show(this.View.Superview, e.Message));
                }
                finally 
                {
                    Utilities.PopNetworkActive();
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

