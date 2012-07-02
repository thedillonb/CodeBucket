using System;
using MonoTouch.UIKit;
using BitbucketBrowser.Utils;
using System.Drawing;
using MonoTouch.Dialog;
using MonoTouch.Foundation;

namespace BitbucketBrowser.UI
{

    public class MultilineElement : CustomElement
    {
        private static float Padding = 6f;
        private static UIFont CaptionFont = UIFont.SystemFontOfSize(14f);

        public MultilineElement(string caption)
            : base(UITableViewCellStyle.Default, "multilineelement")
        {
            this.Caption = caption;
        }

        public override void Draw (RectangleF bounds, MonoTouch.CoreGraphics.CGContext context, UIView view)
        {
            UIColor.Black.SetColor();
            view.DrawString(Caption, new RectangleF(Padding, Padding, bounds.Width - Padding * 2, bounds.Height - Padding * 2), CaptionFont);
        }

        public override float Height (System.Drawing.RectangleF bounds)
        {
            var textHeight = Caption.MonoStringHeight(CaptionFont, bounds.Width - Padding * 2);
            return textHeight + Padding * 2;
        }
    }

    public class StyledElement : MonoTouch.Dialog.StyledStringElement
    {
        private static UIFont TitleFont = UIFont.BoldSystemFontOfSize(14f);
        private static UIFont SubFont = UIFont.SystemFontOfSize(14f);

        public StyledElement(string title, string subtitle, UITableViewCellStyle style)
            : base(title, subtitle, style)
        {
            Font = TitleFont;
            SubtitleFont = SubFont;
        }

        public StyledElement(string title, string subtitle)
            : base(title, subtitle)
        {
            Font = TitleFont;
            SubtitleFont = SubFont;
        }
    }

    public class CustomImageStringElement : ImageStringElement
    {
        private static UIFont Font = UIFont.BoldSystemFontOfSize(16f);
        public CustomImageStringElement (string caption, UIImage image) : base (caption, image)
        {
        }

        public CustomImageStringElement (string caption, string value, UIImage image) 
            : base (caption, value, image)
        {
        }

        public CustomImageStringElement (string caption,  NSAction tapped, UIImage image) 
            : base (caption, tapped, image)
        {
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = base.GetCell(tv);
            cell.TextLabel.Font = Font;
            return cell;
        }
    }
}

