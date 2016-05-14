using System;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels;
using CoreGraphics;
using ReactiveUI;
using UIKit;

namespace CodeBucket.Views
{
    public sealed class EnhancedTableView : ReactiveTableView
    {
        private UIRefreshControl _refreshControl;

        public Lazy<UIView> EmptyView { get; set; }

        private object _viewModel;
        public object ViewModel
        {
            get { return _viewModel; }
            set { this.RaiseAndSetIfChanged(ref _viewModel, value); }
        }

        public UIRefreshControl RefreshControl
        {
            get { return _refreshControl; }
            set
            {
                _refreshControl?.RemoveFromSuperview();
                _refreshControl = value;
                if (_refreshControl != null)
                    AddSubview(_refreshControl);
            }
        }

        public EnhancedTableView(UITableViewStyle style = UITableViewStyle.Plain)
            : base(CGRect.Empty, style)
        {
            this.WhenAnyValue(x => x.ViewModel)
                .OfType<IProvidesSearch>()
                .Subscribe(SetupSearch);

            this.WhenAnyValue(x => x.ViewModel)
                .OfType<ILoadableViewModel>()
                .Subscribe(SetupReload);

            this.WhenAnyValue(x => x.ViewModel)
                .OfType<IPaginatableViewModel>()
                .Subscribe(SetupLoadMore);

            AutosizesSubviews = true;
            CellLayoutMarginsFollowReadableWidth = false;
        }

        public void SetEmpty(bool empty)
        {
            CreateEmptyHandler(empty);
        }

        public void SetupLoadMore(IPaginatableViewModel paginatableViewModel)
        {
            paginatableViewModel.LoadMoreCommand.IsExecuting.Subscribe(x =>
            {
                if (x)
                {
                    var activity = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
                    activity.Frame = new CGRect(0, 0, 320, 64f);
                    activity.StartAnimating();
                    TableFooterView = activity;
                }
                else
                {
                    TableFooterView = null;
                }
            });
        }

        private void SetupSearch(IProvidesSearch searchProvider)
        {
            var searchBar = new UISearchBar(new CGRect(0, 0, Bounds.Width, 44));
            searchBar.Delegate = new SearchDelegate(searchProvider);
            TableHeaderView = searchBar;
        }

        private void SetupReload(ILoadableViewModel loadableViewModel)
        {
            var manualRefreshRequested = false;

            RefreshControl = new UIRefreshControl();
            RefreshControl.GetChangedObservable()
                          .Do(_ => manualRefreshRequested = true)
                          .InvokeCommand(loadableViewModel.LoadCommand);

            loadableViewModel.LoadCommand.IsExecuting.Subscribe(x =>
            {
                if (x)
                {
                    RefreshControl.BeginRefreshing();

                    if (!manualRefreshRequested)
                    {
                        UIView.Animate(0.25, 0f, UIViewAnimationOptions.BeginFromCurrentState | UIViewAnimationOptions.CurveEaseOut,
                            () => ContentOffset = new CGPoint(0, -RefreshControl.Frame.Height), null);
                    }
                }
                else
                {
                    if (RefreshControl.Refreshing)
                    {
                        // Stupid bug...
                        BeginInvokeOnMainThread(() =>
                        {
                            UIView.Animate(0.25, 0.0, UIViewAnimationOptions.BeginFromCurrentState | UIViewAnimationOptions.CurveEaseOut,
                                () => ContentOffset = new CoreGraphics.CGPoint(0, 0), null);
                            RefreshControl.EndRefreshing();
                        });
                    }

                    manualRefreshRequested = false;
                }
            });
        }

        private void CreateEmptyHandler(bool x)
        {
            if (x)
            {
                if (!EmptyView.IsValueCreated)
                {
                    EmptyView.Value.Alpha = 0f;
                    AddSubview(EmptyView.Value);
                }

                EmptyView.Value.UserInteractionEnabled = true;
                EmptyView.Value.Frame = new CGRect(0, 0, Bounds.Width, Bounds.Height);
                SeparatorStyle = UITableViewCellSeparatorStyle.None;
                BringSubviewToFront(EmptyView.Value);
                if (TableHeaderView != null)
                    TableHeaderView.Hidden = true;
                UIView.Animate(0.2f, 0f, UIViewAnimationOptions.AllowUserInteraction | UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.BeginFromCurrentState,
                    () => EmptyView.Value.Alpha = 1.0f, null);
            }
            else if (EmptyView.IsValueCreated)
            {
                EmptyView.Value.UserInteractionEnabled = false;
                if (TableHeaderView != null)
                    TableHeaderView.Hidden = false;
                SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
                UIView.Animate(0.1f, 0f, UIViewAnimationOptions.AllowUserInteraction | UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.BeginFromCurrentState,
                    () => EmptyView.Value.Alpha = 0f, null);
            }
        }

        private class SearchDelegate : UISearchBarDelegate
        {
            readonly WeakReference<IProvidesSearch> _searchProvider;

            public SearchDelegate(IProvidesSearch searchProvider)
            {
                _searchProvider = new WeakReference<IProvidesSearch>(searchProvider);
            }

            public override void OnEditingStarted(UISearchBar searchBar)
            {
                searchBar.ShowsCancelButton = true;
            }

            public override void OnEditingStopped(UISearchBar searchBar)
            {
                searchBar.ShowsCancelButton = false;
            }

            public override void TextChanged(UISearchBar searchBar, string searchText)
            {
                IProvidesSearch search;
                if (_searchProvider.TryGetTarget(out search))
                    search.SearchText = searchText ?? "";
            }

            public override void CancelButtonClicked(UISearchBar searchBar)
            {
                searchBar.ShowsCancelButton = false;
                searchBar.Text = "";
                searchBar.ResignFirstResponder();
            }
        }
    }
}

