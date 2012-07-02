using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace BitbucketBrowser.Utils
{
    class MySearchDelegate : UISearchBarDelegate 
    {
        DialogViewController container;

        public MySearchDelegate (DialogViewController container)
        {
            this.container = container;
        }

        public override void OnEditingStarted (UISearchBar searchBar)
        {
            searchBar.ShowsCancelButton = true;
            container.StartSearch ();
        }

        public override void OnEditingStopped (UISearchBar searchBar)
        {
            searchBar.ShowsCancelButton = false;
            container.FinishSearch ();
        }

        public override void TextChanged (UISearchBar searchBar, string searchText)
        {
            container.PerformFilter (searchText ?? "");
        }

        public override void CancelButtonClicked (UISearchBar searchBar)
        {
            searchBar.ShowsCancelButton = false;
            container.FinishSearch ();
            searchBar.ResignFirstResponder ();
        }

        public override void SearchButtonClicked (UISearchBar searchBar)
        {
            container.SearchButtonClicked (searchBar.Text);
        }
    }
}

