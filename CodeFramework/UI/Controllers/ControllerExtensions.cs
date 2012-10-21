using System;
using MonoTouch.UIKit;
using RedPlum;
using System.Threading;
using MonoTouch;

namespace CodeFramework.UI.Controllers
{
    public static class ControllerExtensions
    {
        public static void DoWork(this UIViewController controller, Action work, Action<Exception> error = null, Action final = null)
        {
            MBProgressHUD hud = null;
            hud = new MBProgressHUD(controller.View.Superview) {Mode = MBProgressHUDMode.Indeterminate, TitleText = "Loading..."};
            controller.View.Superview.AddSubview(hud);
            hud.Show(true);

            ThreadPool.QueueUserWorkItem(delegate {
                try
                {
                    Utilities.PushNetworkActive();
                    work();
                }
                catch (Exception e)
                {
                    if (error != null)
                        controller.InvokeOnMainThread(() => error(e));
                }
                finally 
                {
                    Utilities.PopNetworkActive();
                    if (final != null)
                        controller.InvokeOnMainThread(() => final());
                }
                
                if (hud != null)
                {
                    controller.InvokeOnMainThread(delegate {
                        hud.Hide(true);
                        hud.RemoveFromSuperview();
                    });
                }
            });
        }

        public static void DoWorkNoHud(this UIViewController controller, Action work, Action<Exception> error = null, Action final = null)
        {
            ThreadPool.QueueUserWorkItem(delegate {
                try
                {
                    Utilities.PushNetworkActive();
                    work();
                }
                catch (Exception e)
                {
                    if (error != null)
                        controller.InvokeOnMainThread(() => error(e));
                }
                finally 
                {
                    Utilities.PopNetworkActive();
                    if (final != null)
                        controller.InvokeOnMainThread(() => final());
                }
            });
        }
    }
}

