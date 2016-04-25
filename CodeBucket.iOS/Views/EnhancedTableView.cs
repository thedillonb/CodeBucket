using System;
using CoreGraphics;
using UIKit;

namespace CodeBucket.Views
{
    public class EnhancedTableView : UITableView
    {
        public Lazy<UIView> EmptyView { get; set; }

        public EnhancedTableView(UITableViewStyle style)
            : base(CGRect.Empty, style)
        {
        }

        public void SetEmpty(bool empty)
        {
            CreateEmptyHandler(empty);
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
    }
}

