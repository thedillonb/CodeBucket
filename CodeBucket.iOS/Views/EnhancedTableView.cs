using System;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels;
using CoreGraphics;
using ReactiveUI;
using UIKit;

namespace CodeBucket.Views
{
    public sealed class EnhancedTableView : ReactiveTableView, IActivatable
    {
        private UIRefreshControl _refreshControl;

        public Lazy<UIView> EmptyView { get; set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading == value)
                    return;

                _isLoading = value;
                if (_isLoading)
                {
                    var activity = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
                    activity.Frame = new CGRect(0, 0, 320, 64f);
                    activity.StartAnimating();
                    TableFooterView = activity;
                }
                else
                {
                    TableFooterView = null;
                    ReloadData();
                }
            }
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
            AutosizesSubviews = true;
            CellLayoutMarginsFollowReadableWidth = false;
        }

        private bool _isEmpty;
        public bool IsEmpty
        {
            get { return _isEmpty; }
            set
            {
                if (_isEmpty == value)
                    return;

                _isEmpty = value;
                CreateEmptyHandler(_isEmpty);
            }
        }

        private void CreateEmptyHandler(bool x)
        {
            if (EmptyView == null)
                return;
            
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
    }
}

