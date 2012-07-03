using System;
using System.Drawing;
using MonoTouch.UIKit;

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
}

