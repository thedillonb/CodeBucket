using System;
using System.Threading.Tasks;
using UIKit;
using CodeBucket.Utilities;

namespace CodeBucket.Utilities
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
    }
}

