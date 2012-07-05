using System;
using MonoTouch.UIKit;
using BitbucketBrowser.Utils;
using System.Drawing;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;

namespace BitbucketBrowser.UI
{

    public class MultilineElement : CustomElement
    {
        private static float Padding = 12f;
        private static float PaddingX = 8f;
        private static UIFont CaptionFont = UIFont.SystemFontOfSize(14f);

        public MultilineElement(string caption)
            : base(UITableViewCellStyle.Default, "multilineelement")
        {
            this.Caption = caption;
            BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("/Images/Cells/gradient"));
        }

        public override void Draw (RectangleF bounds, MonoTouch.CoreGraphics.CGContext context, UIView view)
        {
            UIColor.FromRGB(41, 41, 41).SetColor();
            view.DrawString(Caption, new RectangleF(PaddingX, Padding, bounds.Width - Padding * 2, bounds.Height - Padding * 2), CaptionFont);
        }

        public override float Height (System.Drawing.RectangleF bounds)
        {
            var textHeight = Caption.MonoStringHeight(CaptionFont, bounds.Width - PaddingX * 2);
            return textHeight + Padding * 2;
        }
    }

    public class StyledElement : MonoTouch.Dialog.StyledStringElement
    {
        private static UIFont TitleFont = UIFont.BoldSystemFontOfSize(15f);
        private static UIFont SubFont = UIFont.SystemFontOfSize(15f);

        public StyledElement(string title)
            : base(title)
        {
            Init();
        }

        public StyledElement(string title, string subtitle, UITableViewCellStyle style)
            : base(title, subtitle, style)
        {
            Init();
        }

        public StyledElement(string title, string subtitle)
            : base(title, subtitle)
        {
            Init();
        }

        public StyledElement(string title, NSAction action)
            : base(title, action)
        {
            Init();
        }

        public StyledElement (string caption,  NSAction tapped, UIImage image) 
            : base (caption, tapped)
        {
            Init();
            Image = image;
        }

        public StyledElement(string caption, UIImage image)
            : this(caption)
        {
            Init();
            Image = image;
        }


        private void Init()
        {
            Font = TitleFont;
            SubtitleFont = SubFont;
            BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("/Images/Cells/gradient"));
            this.TextColor = UIColor.FromRGB(41, 41, 41);
            this.DetailColor = UIColor.FromRGB(120, 120, 120);
        }


        /*
        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = base.GetCell(tv);
            //cell.TextLabel.BackgroundColor = UIColor.Clear;
            cell.TextLabel.TextColor = TitleColor;

            if (cell.DetailTextLabel != null)
            {
                cell.DetailTextLabel.BackgroundColor = UIColor.Clear;
                cell.DetailTextLabel.TextColor = SubtitleColor;
            }

            return cell;
        }
        */
    }

    public class SubcaptionElement : StyledElement
    {
        private static UIFont TitleFont = UIFont.BoldSystemFontOfSize(15f);
        private static UIFont SubFont = UIFont.SystemFontOfSize(13f);

        public SubcaptionElement(string title, string subtitle)
            : base(title, subtitle, UITableViewCellStyle.Subtitle)
        {
            Font = TitleFont;
            SubtitleFont = SubFont;
        }

        public SubcaptionElement(string title)
            : this(title, null)
        {
        }
    }

    public class ShadowView : UIView
    {
        public ShadowView(float width, float height)
            : base(new RectangleF(0, 0, width, height))
        {
            //BackgroundColor = UIColor.Clear;
        }

        public override void Draw(RectangleF rect)
        {
            var context = UIGraphics.GetCurrentContext();
            using (var cs = CGColorSpace.CreateDeviceRGB ())
            {
                using (var gradient = new CGGradient (cs, new float [] { 0.60f, 0.60f, 0.60f, 0.7f, 0.0f, 0.0f, 0.0f, 0f }, new float [] {0, 1}))
                {
                    context.DrawLinearGradient(gradient, new PointF(Bounds.GetMidX(), 0), new PointF(Bounds.GetMidX(), Bounds.GetMaxY()), 0);
                }
            }
        }
    }


}

