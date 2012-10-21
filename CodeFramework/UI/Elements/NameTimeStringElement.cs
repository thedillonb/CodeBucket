using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Dialog.Utilities;
using MonoTouch.Foundation;
using CodeFramework.UI.Views;

namespace CodeFramework.UI.Elements
{
    public class NameTimeStringElement : CustomElement, IImageUpdated
    {
        public static readonly UIFont DateFont = UIFont.SystemFontOfSize(12);
        public static readonly UIFont UserFont = UIFont.BoldSystemFontOfSize(15);
        public static readonly UIFont DescFont = UIFont.SystemFontOfSize(12);

        public const float LeftRightPadding = 6f;
        public const float TopBottomPadding = 8f;

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
        }

        public override void Draw(RectangleF bounds, CGContext context, UIView view)
        {
            var leftMargin = LeftRightPadding;

            // Superview is the container, its superview the uitableviewcell
            var uiTableViewCell = view.Superview.Superview as UITableViewCell;
            bool highlighted = uiTableViewCell != null && uiTableViewCell.Highlighted & IsTappedAssigned;
            var timeColor = highlighted ? UIColor.White : UIColor.Gray;
            var textColor = highlighted ? UIColor.White : UIColor.FromRGB(41, 41, 41);
            var nameColor = highlighted ? UIColor.White : UIColor.FromRGB(0, 64, 128);

            if (Image != null)
            {
                var imageRect = new RectangleF(LeftRightPadding, TopBottomPadding, 32f, 32f);
                UIColor.White.SetColor ();

                context.SaveState ();
                //context.TranslateCTM (imageRect.X, imageRect.Y);
                context.SetLineWidth (1);
                
                // On device, the shadow is painted in the opposite direction!
                context.SetShadowWithColor (new SizeF (0, 0), 3, UIColor.DarkGray.CGColor);
                context.AddPath (UIBezierPath.FromRect(imageRect).CGPath);
                context.FillPath ();
                context.RestoreState ();

                Image.Draw(imageRect);
                leftMargin += LeftRightPadding + imageRect.Width + 3f;
            }

            var contentWidth = bounds.Width - LeftRightPadding  - leftMargin;


            var daysAgo = DateTime.Parse(Time).ToDaysAgo();
            timeColor.SetColor();
            var daysWidth = daysAgo.MonoStringLength(DateFont);
            RectangleF timeRect;

            timeRect = Image != null ? new RectangleF(leftMargin, TopBottomPadding + UserFont.LineHeight, daysWidth, DateFont.LineHeight) : 
                                       new RectangleF(bounds.Width - LeftRightPadding - daysWidth,  TopBottomPadding + 1f, daysWidth, DateFont.LineHeight);

            view.DrawString(daysAgo, timeRect, DateFont, UILineBreakMode.TailTruncation);

            nameColor.SetColor();
            view.DrawString(Name,
                new RectangleF(leftMargin, TopBottomPadding, contentWidth, UserFont.LineHeight),
                UserFont, UILineBreakMode.TailTruncation
            );


            if (!string.IsNullOrEmpty(String))
            {
                UIColor.Black.SetColor();
                var top = TopBottomPadding + UserFont.LineHeight + 3f;
                if (Image != null)
                    top += DateFont.LineHeight;


                textColor.SetColor();
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

            var desc = String;
            var descHeight = desc.MonoStringHeight(DescFont, contentWidth);
            if (descHeight > (DescFont.LineHeight) * Lines)
                descHeight = (DescFont.LineHeight) * Lines;

            var n = TopBottomPadding*2 + UserFont.LineHeight + 3f + descHeight;
            if (Image != null)
                n += DateFont.LineHeight;
            return n;
        }

        protected override void OnCreateCell(UITableViewCell cell)
        {
            cell.BackgroundView = new CellBackgroundView();
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

