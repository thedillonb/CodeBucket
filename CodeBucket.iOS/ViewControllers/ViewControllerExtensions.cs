using System;
using UIKit;
using Foundation;

namespace CodeBucket.ViewControllers
{
    public static class ViewControllerExtensions
    {
        public static void AddTableView(this BaseViewController controller, UITableView tableView)
        {
            NSObject hideNotification = null, showNotification = null;

            tableView.Frame = controller.View.Bounds;
            tableView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight |
                UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin;
            controller.Add(tableView);

            controller.Appearing.Subscribe(_ =>
            {
                hideNotification = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, notification =>
                {
                    tableView.ContentInset = UIEdgeInsets.Zero;
                    tableView.ScrollIndicatorInsets = UIEdgeInsets.Zero;
                });

                showNotification = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, notification =>
                {
                    var keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);
                    var inset = new UIEdgeInsets(0, 0, keyboardFrame.Height, 0);
                    tableView.ContentInset = inset;
                    tableView.ScrollIndicatorInsets = inset;
                });
            });

            controller.Disappearing.Subscribe(_ =>
            {
                controller.View.EndEditing(true);

                if (hideNotification != null)
                    NSNotificationCenter.DefaultCenter.RemoveObserver(hideNotification);
                if (showNotification != null)
                    NSNotificationCenter.DefaultCenter.RemoveObserver(showNotification);
            });
        }

        public static void PresentModal(this UIViewController presenter, UIViewController presentee)
        {
            if (presentee is WebBrowserViewController)
            {
                presenter.PresentViewController(presentee, true, null);
            }
            else
            {
                var cancelButton = new UIBarButtonItem { Image = Images.Buttons.Cancel };
                cancelButton.GetClickedObservable().Subscribe(_ => presenter.DismissViewController(true, null));
                presentee.NavigationItem.LeftBarButtonItem = cancelButton;
                var nav = new ThemedNavigationController(presentee);
                presenter.PresentViewController(nav, true, null);
            }
        }
    }
}

