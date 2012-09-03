using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;
using System.Collections.Generic;

namespace BitbucketBrowser.UI
{

    public class MultilineElement : CustomElement
    {
        private static float Padding = 12f;
        private static float PaddingX = 8f;
        private static UIFont CaptionFont = UIFont.BoldSystemFontOfSize(15f);
        private static UIFont ValueFont = UIFont.SystemFontOfSize(12f);

        public string Value { get; set; }

        public UIFont PrimaryFont { get; set; }
        public UIColor CaptionColor { get; set; }
        public UIColor ValueColor { get; set; }

        public MultilineElement(string caption)
            : base(UITableViewCellStyle.Default, "multilineelement")
        {
            this.Caption = caption;
            BackgroundColor = UIColor.White;
            PrimaryFont = CaptionFont;
            CaptionColor = ValueColor = UIColor.FromRGB(41, 41, 41);
        }

        public override void Draw(RectangleF bounds, MonoTouch.CoreGraphics.CGContext context, UIView view)
        {
            CaptionColor.SetColor();
            view.DrawString(Caption, new RectangleF(PaddingX, Padding, bounds.Width - Padding * 2, bounds.Height - Padding * 2), PrimaryFont);

            if (Value != null)
            {
                ValueColor.SetColor();
                view.DrawString(Value, new RectangleF(PaddingX, Padding + CaptionFont.LineHeight + 6f, bounds.Width - Padding * 2, bounds.Height), ValueFont, UILineBreakMode.WordWrap);
            }
        }


        public override float Height(System.Drawing.RectangleF bounds)
        {
            var textHeight = Caption.MonoStringHeight(PrimaryFont, bounds.Width - PaddingX * 2);

            if (Value != null)
            {
                textHeight += 6f;
                textHeight += Value.MonoStringHeight(ValueFont, bounds.Width - PaddingX * 2);
            }

            return textHeight + Padding * 2;
        }
    }

    public class NoItemsElement : StyledElement
    {
        public NoItemsElement()
            : base("No Items")
        {
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            var c = base.GetCell(tv);
            c.TextLabel.TextAlignment = UITextAlignment.Center;
            return c;
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
            Accessory = UITableViewCellAccessory.DisclosureIndicator;
        }

        public StyledElement (string caption,  NSAction tapped, UIImage image) 
            : base (caption, tapped)
        {
            Init();
            Image = image;
            Accessory = UITableViewCellAccessory.DisclosureIndicator;
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
            BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("/Images/TableCell"));
            this.TextColor = UIColor.FromRGB(41, 41, 41);
            this.DetailColor = UIColor.FromRGB(120, 120, 120);
            LineBreakMode = MonoTouch.UIKit.UILineBreakMode.TailTruncation;
            Lines = 1;
        }
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

    public class DropbarElement : UIView
    {
        private static UIImage Image = UIImage.FromBundle("/Images/Dropbar");

        public DropbarElement(float width)
            : base (new RectangleF(0, 0, width, Image.Size.Height))
        {
            BackgroundColor = UIColor.FromPatternImage(Image);
            Layer.MasksToBounds = false;
            Layer.ShadowColor = UIColor.Black.CGColor;
            Layer.ShadowOpacity = 0.3f;
            Layer.ShadowOffset = new SizeF(0, 5f);
        }
    }
}

