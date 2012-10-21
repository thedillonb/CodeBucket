using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch;
using CodeFramework.UI.Views;
using CodeFramework.UI.Elements;

namespace CodeFramework.UI.Controllers
{
    public abstract class Controller<T> : BaseDialogViewController where T : class
    {
        public T Model { get; set; }
        public bool Loaded { get; private set; }
        protected ErrorView CurrentError;
        private SearchFilterBar _searchBar;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name='push'>True if navigation controller should push, false if otherwise</param>
        /// <param name='refresh'>True if the data can be refreshed, false if otherwise</param>
        protected Controller(bool push = false, bool refresh = false)
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
            _searchBar = new SearchFilterBar {Delegate = new CustomSearchDelegate<T>(this)};
            //searchBar.FilterButton.TouchUpInside += FilterButtonTouched;
            return _searchBar;
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
                if (CurrentError != null)
                    CurrentError.RemoveFromSuperview();
                CurrentError = null;
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
                        TableView.TableFooterView.Hidden = Root.Count == 0;

                    ReloadComplete(); 
                });
                Loaded = true;
                return;
            }

            if (!force)
            {
                this.DoWork(() => UpdateAndRefresh(false), ex => {
                    CurrentError = ErrorView.Show(View.Superview, ex.Message);
                });
            }
            else
            {
                this.DoWorkNoHud(() => UpdateAndRefresh(true), ex => Utilities.ShowAlert("Unable to refresh!", "There was an issue while attempting to refresh. " + ex.Message), ReloadComplete);
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
            _searchBar.FilterButtonVisible = false;
        }
        
        protected virtual void SearchEnd()
        {
            _searchBar.FilterButtonVisible = true;
        }
        
        class CustomSearchDelegate<T> : UISearchBarDelegate where T : class
        {
            readonly Controller<T> _container;
            DialogViewController _searchController;
            List<ElementContainer> _searchElements;
            
            static UIColor NoItemColor = UIColor.FromRGBA(0.1f, 0.1f, 0.1f, 0.9f);
            
            class ElementContainer
            {
                public Element Element;
                public Element Parent;
            }
            
            public CustomSearchDelegate (Controller<T> container)
            {
                _container = container;
            }
            
            public override void OnEditingStarted (UISearchBar searchBar)
            {
                _container.SearchStart();
                
                if (_searchController == null)
                {
                    _searchController = new DialogViewController(UITableViewStyle.Plain, null);
                    _searchController.LoadView();
                    _searchController.TableView.TableFooterView = new DropbarView(1f);
                }
                
                searchBar.ShowsCancelButton = true;
                _container.TableView.ScrollRectToVisible(new RectangleF(0, 0, 1, 1), false);
                _container.NavigationController.SetNavigationBarHidden(true, true);
                _container.IsSearching = true;
                _container.TableView.ScrollEnabled = false;
                
                if (_searchController.Root != null && _searchController.Root.Count > 0 && _searchController.Root[0].Count > 0)
                {
                    _searchController.TableView.TableFooterView.Hidden = false;
                    _searchController.View.BackgroundColor = UIColor.White;
                    _searchController.TableView.ScrollEnabled = true;
                }
                else
                {
                    _searchController.TableView.TableFooterView.Hidden = true;
                    _searchController.View.BackgroundColor = NoItemColor;
                    _searchController.TableView.ScrollEnabled = false;
                }
                
                _searchElements = new List<ElementContainer>();
                
                //Grab all the elements that we could search trhough
                foreach (var s in _container.Root)
                    foreach (var e in s.Elements)
                        _searchElements.Add(new ElementContainer { Element = e, Parent = e.Parent });
                
                if (!_container.ChildViewControllers.Contains(_searchController))
                {
                    _searchController.View.Frame = new RectangleF(_container.TableView.Bounds.X, 44f, _container.TableView.Bounds.Width, _container.TableView.Bounds.Height - 44f);
                    _container.AddChildViewController(_searchController);
                    _container.View.AddSubview(_searchController.View);
                }
                
                
            }
            
            public override void OnEditingStopped (UISearchBar searchBar)
            {
                
            }
            
            public override void TextChanged (UISearchBar searchBar, string searchText)
            {
                if (string.IsNullOrEmpty(searchText))
                {
                    if (_searchController.Root != null)
                        _searchController.Root.Clear();
                    _searchController.View.BackgroundColor = NoItemColor;
                    _searchController.TableView.TableFooterView.Hidden = true;
                    _searchController.TableView.ScrollEnabled = false;
                    return;
                }
                
                var sec = new Section();
                foreach (var el in _searchElements)
                {
                    if (el.Element.Matches(searchText))
                    {
                        sec.Add(el.Element);
                    }
                }
                _searchController.TableView.ScrollEnabled = true;
                
                if (sec.Count == 0)
                {
                    sec.Add(new NoItemsElement());
                }
                
                _searchController.View.BackgroundColor = UIColor.White;
                _searchController.TableView.TableFooterView.Hidden = sec.Count == 0;
                var root = new RootElement("") { sec };
                root.UnevenRows = true;
                _searchController.Root = root;
            }
            
            public override void CancelButtonClicked (UISearchBar searchBar)
            {
                //Reset the parent
                foreach (var s in _searchElements)
                    s.Element.Parent = s.Parent;
                
                searchBar.Text = "";
                searchBar.ShowsCancelButton = false;
                _container.FinishSearch ();
                searchBar.ResignFirstResponder ();
                _container.NavigationController.SetNavigationBarHidden(false, true);
                _container.IsSearching = false;
                _container.TableView.ScrollEnabled = true;
                
                _searchController.RemoveFromParentViewController();
                _searchController.View.RemoveFromSuperview();
                
                if (_searchController.Root != null)
                    _searchController.Root.Clear();
                
                _searchElements.Clear();
                _searchElements = null;
                
                _container.SearchEnd();
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

