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
    public abstract class Controller<T> : BaseDialogViewController
    {
        public T Model { get; set; }
        public bool Loaded { get; private set; }
        protected ErrorView _currentError;
        private SearchFilterBar searchBar;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFramework.UI.Controllers.Controller`1"/> class.
        /// </summary>
        /// <param name='push'>True if navigation controller should push, false if otherwise</param>
        /// <param name='refresh'>True if the data can be refreshed, false if otherwise</param>
        public Controller(bool push = false, bool refresh = false)
            : base(push)
        {
            if (refresh)
                RefreshRequested += (sender, e) => Refresh(true);
        }

        //Called when the UI needs to be updated with the model data            
        protected abstract void OnRefresh();

        //Called when the controller needs to request the model from the server
        protected abstract T OnUpdate(bool forced);

        //Filter the items!
        protected virtual T OnOrder(T item)
        {
            return item;
        }

        protected override UISearchBar CreateHeaderView()
        {
            searchBar = new SearchFilterBar();
            searchBar.Delegate = new CustomSearchDelegate(this);
            searchBar.FilterButton.TouchUpInside += FilterButtonTouched;
            return searchBar;
        }

        void FilterButtonTouched (object sender, EventArgs e)
        {
            var filter = new FilterController();
            filter.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, (s, a) => { 
                filter.DismissModalViewControllerAnimated(true); 
            });
            var nav = new UINavigationController(filter);
            this.PresentModalViewController(nav, true);
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
                    Model = OnOrder(Model);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("There was an issue attempting to filter: " + ex.Message);
                }

                try
                {
                    OnRefresh();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("There was an issue attempting to refresh: " + ex.Message);
                }

                InvokeOnMainThread(delegate { 
                    if (TableView.TableFooterView != null)
                        TableView.TableFooterView.Hidden = this.Root.Count == 0;

                    ReloadComplete(); 
                });
                Loaded = true;
                return;
            }

            if (!force)
            {
                this.DoWork(() => UpdateAndRefresh(force), (ex) => {
                    _currentError = ErrorView.Show(this.View.Superview, ex.Message);
                });
            }
            else
            {
                this.DoWorkNoHud(() => UpdateAndRefresh(force), (ex) => {
                    Utilities.ShowAlert("Unable to refresh!", "There was an issue while attempting to refresh. " + ex.Message);
                }, ReloadComplete);
            }
        }

        private void UpdateAndRefresh(bool force)
        {
            Model = OnUpdate(force);
            if (Model != null)
                Refresh();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (!Loaded)
            {
                Refresh();
                Loaded = true;
            }
        }

        protected virtual void SearchStart()
        {
            searchBar.FilterButtonVisible = false;
        }
        
        protected virtual void SearchEnd()
        {
            searchBar.FilterButtonVisible = true;
        }
        
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

    }
}

