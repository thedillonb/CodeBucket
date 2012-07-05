using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace BitbucketBrowser.UI
{
    public class FirstElement : CustomElement
    {
        public string Caption { get; set; }
        public string Time { get; set; }

        public FirstElement()
            : base(UITableViewCellStyle.Default, "firstelement")
        {
        }


        public override void Draw(RectangleF bounds, MonoTouch.CoreGraphics.CGContext context, UIView view)
        {

        }

        public override float Height(RectangleF bounds)
        {
            return 35f;
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = base.GetCell(tv);
            cell.BackgroundColor = UIColor.FromRGB(180, 180, 180);
            return cell;
        }

    }


    public class SeperatorElement : CustomElement
    {
        private static UIFont Font = UIFont.BoldSystemFontOfSize(14f);

        public SeperatorElement(string title)
            : base(UITableViewCellStyle.Default, "seperatorelement")
        {
            Caption = title;
            BackgroundColor = UIColor.Clear;
        }

        public override void Draw(RectangleF bounds, MonoTouch.CoreGraphics.CGContext context, UIView view)
        {
            /*
            using (var cs = CGColorSpace.CreateDeviceRGB ())
            {
                using (var gradient = new CGGradient (cs, new float [] { 0.88f, 0.88f, 0.88f, 1.0f, 0.9f, 0.9f, 0.9f, 1.0f, 0.88f, 0.88f, 0.88f, 1.0f }, new float [] {0f, 0.5f, 1f}))
                {
                    context.DrawLinearGradient(gradient, new PointF(0, 0), new PointF(0, bounds.Height), 0);
                }
            }
            */

            UIColor.White.SetColor();
            view.DrawString(Caption, new RectangleF(13f, bounds.Height / 2 - 8f, bounds.Width - 26f, bounds.Height), Font);
        }

        public override float Height(RectangleF bounds)
        {
            return 30f;
        }
    }
}

