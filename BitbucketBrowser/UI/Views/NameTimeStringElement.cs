using System;
using MonoTouch.UIKit;
using System.Drawing;
using BitbucketBrowser.Utils;
using MonoTouch.CoreGraphics;
using MonoTouch.Dialog.Utilities;
using MonoTouch.Foundation;

namespace BitbucketBrowser.UI
{
public class NameTimeStringElement : CustomElement, IImageUpdated
    {
        private static readonly UIFont DateFont = UIFont.SystemFontOfSize(12);
        private static readonly UIFont UserFont = UIFont.BoldSystemFontOfSize(15);
        private static readonly UIFont DescFont = UIFont.SystemFontOfSize(14);

        private const float LeftRightPadding = 6f;
        private const float TopBottomPadding = 6f;

        public string Name { get; set; }
        public string Time { get; set; }
        public string String { get; set; }
        public Uri ImageUri { get; set; }

        public int Lines { get; set; }
        public UIImage Image { get; set; }

        public NameTimeStringElement() 
            : base(UITableViewCellStyle.Default, "nametimestringelement")
        {
            Lines = 9999;
            BackgroundColor = UIColor.FromPatternImage(Images.CellGradient);
        }

        public override void Draw(RectangleF bounds, CGContext context, UIView view)
        {
            var leftMargin = LeftRightPadding;

            if (Image != null)
            {
                var imageRect = new RectangleF(LeftRightPadding, TopBottomPadding, 32f, 32f);
                Image.Draw(imageRect);
                leftMargin += LeftRightPadding + imageRect.Width;
            }

            var contentWidth = bounds.Width - LeftRightPadding  - leftMargin;


            var daysAgo = DateTime.Parse(Time).ToDaysAgo();
            UIColor.FromRGB(0.6f, 0.6f, 0.6f).SetColor();
            var daysWidth = daysAgo.MonoStringLength(DateFont);
            RectangleF timeRect;

            if (Image != null)
                timeRect = new RectangleF(leftMargin, TopBottomPadding + UserFont.LineHeight, daysWidth, DateFont.LineHeight);
            else
                timeRect = new RectangleF(bounds.Width - LeftRightPadding - daysWidth,  TopBottomPadding + 1f, daysWidth, DateFont.LineHeight);

            view.DrawString(daysAgo, timeRect, DateFont, UILineBreakMode.TailTruncation);


            var nameWidth = contentWidth;
            if (Image == null)
                nameWidth -= daysWidth;
            UIColor.FromRGB(0, 64, 128).SetColor();
            view.DrawString(Name,
                new RectangleF(leftMargin, TopBottomPadding, contentWidth, UserFont.LineHeight),
                UserFont, UILineBreakMode.TailTruncation
            );


            if (!string.IsNullOrEmpty(String))
            {
                UIColor.Black.SetColor();
                var top = TopBottomPadding + UserFont.LineHeight + 1f;
                if (Image != null)
                    top += DateFont.LineHeight;


                UIColor.FromRGB(41, 41, 41).SetColor();
                view.DrawString(String,
                    new RectangleF(LeftRightPadding, top, bounds.Width - LeftRightPadding*2, bounds.Height - TopBottomPadding - top), DescFont, UILineBreakMode.TailTruncation
                );
            }
        }

        public override float Height(RectangleF bounds)
        {
            var contentWidth = bounds.Width - LeftRightPadding * 2; //Account for the Accessory
            if (IsTappedAssigned)
                contentWidth -= 20f;

            var desc = this.String;
            var descHeight = desc.MonoStringHeight(DescFont, contentWidth);
            if (descHeight > (DescFont.LineHeight) * Lines)
                descHeight = (DescFont.LineHeight) * Lines;

            var n = TopBottomPadding*2 + UserFont.LineHeight + 2f + descHeight;
            if (Image != null)
                n += DateFont.LineHeight;
            return n;
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            if (ImageUri != null)
            {
                var img = ImageLoader.DefaultRequestImage(ImageUri, this);
                if (img != null)
                    Image = img;
            }

            return base.GetCell(tv);
        }

        void IImageUpdated.UpdatedImage (Uri uri)
        {
            var img = ImageLoader.DefaultRequestImage(uri, this);
            Image = img;

            if (uri == null)
                return;
            var root = GetImmediateRootElement ();
            if (root == null || root.TableView == null)
                return;
            root.TableView.ReloadRows (new NSIndexPath [] { IndexPath }, UITableViewRowAnimation.None);
        }
    }
}

