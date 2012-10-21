using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeFramework.UI.Views;
using System.Drawing;

namespace CodeFramework.UI.Controllers
{
    public class BaseDialogViewController : DialogViewController
    {
        protected bool IsSearching;

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (NavigationController != null && IsSearching)
                NavigationController.SetNavigationBarHidden(true, true);
            if (IsSearching)
            {
                TableView.ScrollRectToVisible(new RectangleF(0, 0, 1, 1), false);
            }
        }
        
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (NavigationController != null && NavigationController.NavigationBarHidden)
                NavigationController.SetNavigationBarHidden(false, true);
            
            if (IsSearching)
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
            if (Title != null && Root != null)
                Root.Caption = Title;

            TableView.BackgroundColor = UIColor.Clear;
            TableView.BackgroundView = null;
            if (Style != UITableViewStyle.Grouped)
            {
                TableView.TableFooterView = new DropbarView(View.Bounds.Width) {Hidden = true};
            }

            WatermarkView.AssureWatermark(this);
            base.ViewDidLoad();
        }

        sealed class RefreshView : RefreshTableHeaderView
        {
            public RefreshView(RectangleF rect)
                : base(rect)
            {
                BackgroundColor = UIColor.Clear;
                StatusLabel.BackgroundColor = UIColor.Clear;
                LastUpdateLabel.BackgroundColor = UIColor.Clear;
            }
        }
    }
}

