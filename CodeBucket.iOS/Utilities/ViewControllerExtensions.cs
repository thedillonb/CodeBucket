using System;
using System.Threading.Tasks;
using UIKit;
using CodeBucket.Utilities;
using Foundation;

namespace CodeBucket.ViewControllers
{
    public static class ViewControllerExtensions
    {
        public async static Task DoWorkAsync(this UIViewController controller, string workTitle, Func<Task> work)
        {
            var hud = new Hud();
			hud.Show(workTitle);

            //Make sure the Toolbar is disabled too
            if (controller.ToolbarItems != null)
            {
                foreach (var t in controller.ToolbarItems)
                    t.Enabled = false;
            }

            try
            {
                NetworkActivity.PushNetworkActive();
                await work();
            }
            finally
            {
                NetworkActivity.PopNetworkActive();
                hud.Hide();

                //Enable all the toolbar items
                if (controller.ToolbarItems != null)
                {
                    foreach (var t in controller.ToolbarItems)
                        t.Enabled = true;
                }
            }
        }

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
    }
}

