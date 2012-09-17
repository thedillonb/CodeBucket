using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeFramework.UI.Views;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using CodeFramework.UI.Elements;

namespace CodeFramework.UI.Controllers
{
    public class BaseDialogViewController : DialogViewController
    {
        protected bool isSearching = false;

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

        public BaseDialogViewController(bool push)
            : base(new RootElement(""), push)
        {
            NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);
            SearchPlaceholder = "Search";
            Autorotate = true;
        }
        
        /// <summary>
        /// Makes the refresh table header view.
        /// </summary>
        /// <returns>
        /// The refresh table header view.
        /// </returns>
        /// <param name='rect'>
        /// Rect.
        /// </param>
        public override RefreshTableHeaderView MakeRefreshTableHeaderView(RectangleF rect)
        {
            //Replace it with our own
            return new RefreshView(rect);
        }
        
        public override void ViewDidLoad()
        {
            var search = this.TableView.TableHeaderView as UISearchBar;
            if (search != null)
                search.Delegate = new CustomSearchDelegate(this);

            if (Title != null && Root != null)
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
        
        protected virtual void SearchStart()
        {
        }

        protected virtual void SearchEnd()
        {
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
        }
        
        
        class CustomSearchDelegate : UISearchBarDelegate 
        {
            BaseDialogViewController container;
            DialogViewController searchController;
            List<ElementContainer> searchElements;
            
            static UIColor NoItemColor = UIColor.FromRGBA(0.1f, 0.1f, 0.1f, 0.9f);
            
            class ElementContainer
            {
                public Element Element;
                public Element Parent;
            }
            
            public CustomSearchDelegate (BaseDialogViewController container)
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

