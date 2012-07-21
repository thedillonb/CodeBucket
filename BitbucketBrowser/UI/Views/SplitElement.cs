using System;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;

namespace BitbucketBrowser.UI
{
  public class SplitElement : CustomElement
    {
        private readonly List<Row> _rows;
        private static UIFont Font = UIFont.SystemFontOfSize(12f);

        public List<Row> Rows { get { return _rows; } }

        public SplitElement(IEnumerable<Row> rows)
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

                if (!string.IsNullOrEmpty(row.Text1))
                {
                    UIColor.Gray.SetColor();
                    view.DrawString(row.Text1, new RectangleF(36,  10 * (i + 1) + i * 28, bounds.Width / 2 - 40, Font.LineHeight), Font, UILineBreakMode.TailTruncation);
                }

                row.Image2.Draw(new RectangleF(bounds.Width / 2 + 15, 10 * (i + 1) + i * 28, 16f, 16f));

                if (!string.IsNullOrEmpty(row.Text2))
                {
                    UIColor.Gray.SetColor();
                    view.DrawString(row.Text2, new RectangleF(bounds.Width / 2 + 36,  10 * (i + 1) + i * 28, bounds.Width / 2 - 40, Font.LineHeight), Font, UILineBreakMode.TailTruncation);
                }


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

