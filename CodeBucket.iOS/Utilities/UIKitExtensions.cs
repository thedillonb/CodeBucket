using System;
using System.Reactive.Linq;
using System.Reactive;
using CoreGraphics;
using ReactiveUI;
using System.Reactive.Disposables;

// Analysis disable once CheckNamespace
namespace UIKit
{
    public static class UIKitExtensions
    {
        public static IDisposable Bind(this UIBarButtonItem @this, ReactiveCommand<Unit, Unit> cmd)
        {
            var d1 = @this.GetClickedObservable().SelectUnit().BindCommand(cmd);
            var d2 = cmd.CanExecute.Subscribe(x => @this.Enabled = x);
            return new CompositeDisposable(d1, d2);
        }

        public static IObservable<int> GetChangedObservable(this UISegmentedControl @this)
        {
            return Observable.FromEventPattern(t => @this.ValueChanged += t, t => @this.ValueChanged -= t).Select(_ => (int)@this.SelectedSegment);
        }

        public static IObservable<string> GetChangedObservable(this UITextField @this)
        {
            return Observable.FromEventPattern(t => @this.EditingChanged += t, t => @this.EditingChanged -= t).Select(_ => @this.Text);
        }

        public static IObservable<string> GetChangedObservable(this UITextView @this)
        {
            return Observable.FromEventPattern(t => @this.Changed += t, t => @this.Changed -= t).Select(_ => @this.Text);
        }

        public static IObservable<Unit> GetClickedObservable(this UIButton @this)
        {
            return Observable.FromEventPattern(t => @this.TouchUpInside += t, t => @this.TouchUpInside -= t).Select(_ => Unit.Default);
        }

        public static IObservable<UIBarButtonItem> GetClickedObservable(this UIBarButtonItem @this)
        {
            return Observable.FromEventPattern(t => @this.Clicked += t, t => @this.Clicked -= t).Select(_ => @this);
        }

        public static IObservable<Unit> GetChangedObservable(this UIRefreshControl @this)
        {
            return Observable.FromEventPattern(t => @this.ValueChanged += t, t => @this.ValueChanged -= t).Select(_ => Unit.Default);
        }

        public static IObservable<string> GetChangedObservable(this UISearchBar @this)
        {
            return Observable.FromEventPattern<UISearchBarTextChangedEventArgs>(t => @this.TextChanged += t, t => @this.TextChanged -= t).Select(_ => @this.Text);
        }

        public static IObservable<Unit> GetSearchObservable(this UISearchBar @this)
        {
            return Observable.FromEventPattern(t => @this.SearchButtonClicked += t, t => @this.SearchButtonClicked -= t).Select(_ => Unit.Default);
        }

        public static IObservable<Unit> GetCanceledObservable(this UISearchBar @this)
        {
            return Observable.FromEventPattern(t => @this.CancelButtonClicked += t, t => @this.CancelButtonClicked -= t).Select(_ => Unit.Default);
        }

        public static UISearchBar CreateSearchBar(this UITableView tableView)
        {
            var searchBar = new UISearchBar(new CGRect(0, 0, tableView.Bounds.Width, 44));
            searchBar.OnEditingStarted += (sender, e) => searchBar.ShowsCancelButton = true;
            searchBar.OnEditingStopped += (sender, e) => searchBar.ShowsCancelButton = false;
            searchBar.CancelButtonClicked += (sender, e) =>
            {
                searchBar.ShowsCancelButton = false;
                searchBar.Text = "";
                searchBar.ResignFirstResponder();
            };
            tableView.TableHeaderView = searchBar;
            return searchBar;
        }

        public static void DisposeAll(this UIView view)
        {
            if (view.IsDisposedOrNull())
                return;

            var tableView = view as UITableView;
            if (tableView != null)
            {
                tableView.Source?.Dispose();
                tableView.Source = null;
            }

            try
            {
                foreach (var subView in view.Subviews)
                    subView.DisposeAll();

                view.RemoveFromSuperview();
                view.Dispose();
            }
            catch
            {
            }
        }

        public static bool IsDisposedOrNull(this UIView view)
        {
            if (view == null) return true;
            if (view.Handle == IntPtr.Zero) return true;
            return false;
        }
    }

    public static class UIFontExtensions
    {
        public static UIFont MakeBold(this UIFont font)
        {
            return UIFont.FromDescriptor(font.FontDescriptor.CreateWithTraits(UIFontDescriptorSymbolicTraits.Bold), font.PointSize);
        }

        public static UIFont MakeItalic(this UIFont font)
        {
            return UIFont.FromDescriptor(font.FontDescriptor.CreateWithTraits(UIFontDescriptorSymbolicTraits.Italic), font.PointSize);
        }
    }

}

