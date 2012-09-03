using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace CodeFramework.UI.Views
{
    public class WatermarkView : UIView
    {
        public static UIImage Image;

        public static void AssureWatermark(UIViewController controller)
        {
            if (Image == null)
                return;

            if (controller.ParentViewController != null)
            {
                controller.ParentViewController.View.BackgroundColor = UIColor.FromPatternImage(Image);
            }
        }
    }
}

