using System;
using System.Threading;
using System.Threading.Tasks;
using MonoTouch;
using UIKit;

namespace CodeBucket.Utils
{
    public static class ViewControllerExtensions
    {
		public static IHud CreateHud(this UIViewController controller)
		{
			return new Hud(controller.View);
		}

        public async static Task DoWorkAsync(this UIViewController controller, string workTitle, Func<Task> work)
        {
			var hud = CreateHud(controller);
			hud.Show(workTitle);

            //Make sure the Toolbar is disabled too
            if (controller.ToolbarItems != null)
            {
                foreach (var t in controller.ToolbarItems)
                    t.Enabled = false;
            }

            try
            {
                await DoWorkNoHudAsync(controller, work);
            }
            finally
            {
                hud.Hide();

                //Enable all the toolbar items
                if (controller.ToolbarItems != null)
                {
                    foreach (var t in controller.ToolbarItems)
                        t.Enabled = true;
                }
            }
        }

        public async static Task DoWorkNoHudAsync(this UIViewController controller, Func<Task> work)
        {
            try
            {
                Utilities.PushNetworkActive();
                await work();
            }
            catch (Exception e)
            {
                Utilities.LogException(e.Message, e);
                throw e;
            }
            finally 
            {
                Utilities.PopNetworkActive();
            }
        }
    }
}

