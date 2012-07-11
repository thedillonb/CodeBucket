using System;
using MonoTouch.UIKit;
using BitbucketBrowser.Utils;
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
        private static UIFont CaptionFont = UIFont.SystemFontOfSize(15f);
        private static UIFont ValueFont = UIFont.SystemFontOfSize(12f);

        public string Value { get; set; }

        public MultilineElement(string caption)
            : base(UITableViewCellStyle.Default, "multilineelement")
        {
            this.Caption = caption;
            BackgroundColor = UIColor.FromRGB(246, 247, 248);

        }

        public override void Draw(RectangleF bounds, MonoTouch.CoreGraphics.CGContext context, UIView view)
        {
            UIColor.FromRGB(41, 41, 41).SetColor();
            view.DrawString(Caption, new RectangleF(PaddingX, Padding, bounds.Width - Padding * 2, bounds.Height - Padding * 2), CaptionFont);

            if (Value != null)
            {
                UIColor.FromRGB(120, 120, 120).SetColor();
                view.DrawString(Value, new RectangleF(PaddingX, Padding + CaptionFont.LineHeight + 7f, bounds.Width - Padding * 2, bounds.Height), ValueFont, UILineBreakMode.WordWrap);
            }
        }

        public override float Height(System.Drawing.RectangleF bounds)
        {
            var textHeight = Caption.MonoStringHeight(CaptionFont, bounds.Width - PaddingX * 2);

            if (Value != null)
            {
                textHeight += 6f;
                textHeight += Value.MonoStringHeight(ValueFont, bounds.Width - PaddingX * 2);
            }

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
            BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("/Images/TableCell"));
            this.TextColor = UIColor.FromRGB(41, 41, 41);
            this.DetailColor = UIColor.FromRGB(120, 120, 120);
        }



        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = base.GetCell(tv);
            cell.TextLabel.BackgroundColor = UIColor.Clear;

            if (cell.DetailTextLabel != null)
                cell.DetailTextLabel.BackgroundColor = UIColor.Clear;

            return cell;
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


    public class RepositoryInfo : CustomElement
    {
        private readonly List<Row> _rows;
        private static UIFont Font = UIFont.SystemFontOfSize(12f);

        public RepositoryInfo(IEnumerable<Row> rows)
            : base(UITableViewCellStyle.Default, "repoinfo")
        {
            _rows = new List<Row>(rows);
            BackgroundColor = UIColor.FromRGB(249, 250, 251); //UIColor.FromPatternImage(UIImage.FromBundle("/Images/Cells/gradient"));
        }

        public class Row
        {
            public string Text1 { get; set; }
            public UIImage Image1 { get; set; }

            public string Text2 { get; set; }
            public UIImage Image2 { get; set; }
        }


        public override float Height(RectangleF bounds)
        {
            return _rows.Count * 36 + (_rows.Count - 1) * 2;
        }

        public override void Draw(RectangleF bounds, CGContext context, UIView view)
        {
            context.SetLineWidth(1);

            context.BeginPath();
            context.SetStrokeColor(UIColor.FromRGBA(205, 205, 205, 128).CGColor);
            context.MoveTo(bounds.Width / 2 - 0.5f, 0f);
            context.AddLineToPoint(bounds.Width / 2 - 0.5f, bounds.Height);
            context.StrokePath();

            /*
            context.BeginPath();
            context.SetStrokeColor(UIColor.FromRGBA(250, 250, 250, 128).CGColor);
            context.MoveTo(bounds.Width / 2 + 0.5f, 0);
            context.AddLineToPoint(bounds.Width / 2 + 0.5f, bounds.Height);
            context.StrokePath();
            */

            for (int i = 0; i < _rows.Count; i++)
            {
                var row = _rows[i];

                row.Image1.Draw(new RectangleF(15, 10 * (i + 1) + i * 28, 16f, 16f));

                UIColor.Gray.SetColor();
                view.DrawString(row.Text1, new RectangleF(36,  10 * (i + 1) + i * 28, bounds.Width / 2 - 40, Font.LineHeight), Font, UILineBreakMode.TailTruncation);


                row.Image2.Draw(new RectangleF(bounds.Width / 2 + 15, 10 * (i + 1) + i * 28, 16f, 16f));

                UIColor.Gray.SetColor();
                view.DrawString(row.Text2, new RectangleF(bounds.Width / 2 + 36,  10 * (i + 1) + i * 28, bounds.Width / 2 - 40, Font.LineHeight), Font, UILineBreakMode.TailTruncation);



                //Last item
                if (i < _rows.Count - 1)
                {
                    var y = 36f + i * 38f;

                    context.BeginPath();
                    context.SetStrokeColor(UIColor.FromRGBA(205, 205, 205, 128).CGColor);
                    context.MoveTo(0, y - 0.5f);
                    context.AddLineToPoint(bounds.Width, y - 0.5f);
                    context.StrokePath();

                    context.BeginPath();
                    context.SetStrokeColor(UIColor.FromRGBA(250, 250, 250, 128).CGColor);
                    context.MoveTo(0, (y + 1) - 0.5f);
                    context.AddLineToPoint(bounds.Width, (y + 1) - 0.5f);
                    context.StrokePath();
                }
            }
        }
    }


}

