using System;
using System.Reactive.Linq;
using System.Reactive;
using ReactiveUI;
using System.Reactive.Disposables;
using CoreGraphics;


// Analysis disable once CheckNamespace
namespace UIKit
{
    public static class UIKitExtensions
    {
        public static IDisposable BindCommand<T>(this UIBarButtonItem @this, IReactiveCommand<T> cmd)
        {
            var invoke = @this.GetClickedObservable().InvokeCommand(cmd);
            var canExecute = cmd.CanExecuteObservable.Subscribe(x => @this.Enabled = x);
            return new CompositeDisposable(invoke, canExecute);
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
            return searchBar;
        }

        public static void DisposeAll(this UIView view)
        {
            if (view.IsDisposedOrNull())
                return;
            
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
}

