using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace BitbucketBrowser.UI
{
    public class WatermarkView : UIView
    {
        public WatermarkView(RectangleF frame, UIColor color, UIImage Watermark)
            : base(frame)
        {
            this.BackgroundColor = color;

            //Add the watermark
            var watermark = new UIImageView(Images.Watermark);
            watermark.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;

            watermark.Frame = new RectangleF(this.Frame.Width / 2 - Images.Watermark.Size.Width / 2, 
                     this.Frame.Height / 2 - Images.Watermark.Size.Height / 2, 
                     Images.Watermark.Size.Width,
                     Images.Watermark.Size.Height);

            this.InsertSubview(watermark, 0);

            this.AutoresizingMask = UIViewAutoresizing.All;
            this.AutosizesSubviews = true;
        }

        public static void AssureWatermark(UIViewController controller)
        {
            if (controller.ParentViewController != null)
            {
                bool hasSubview = false;
                foreach (var s in controller.ParentViewController.View.Subviews)
                {
                    if (s is WatermarkView)
                    {
                        hasSubview = true;
                        break;
                    }
                }


                if (!hasSubview)
                {
                    controller.ParentViewController.View.InsertSubview(new WatermarkView(controller.View.Frame, UIColor.FromPatternImage(Images.Background), Images.Watermark), 0);
                }

            }
        }
    }
}

