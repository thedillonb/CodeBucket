using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;

namespace CodeFramework.UI.Elements
{
  public class SplitElement : CustomElement
    {
        private static readonly UIFont Font = UIFont.SystemFontOfSize(12f);

        public Row Value { get; set; }

        public SplitElement(Row row)
            : base(UITableViewCellStyle.Default, "repoinfo")
        {
            Value = row;
            BackgroundColor = UIColor.FromRGB(249, 250, 251);
        }

        public class Row
        {
            public string Text1, Text2;
            public UIImage Image1, Image2;
        }


        public override float Height(RectangleF bounds)
        {
            return 34f;
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

            var row = Value;
            var half = bounds.Height / 2;
            var halfText = Font.LineHeight / 2 + 1;

            row.Image1.Draw(new RectangleF(15, half - 8f, 16f, 16f));

            if (!string.IsNullOrEmpty(row.Text1))
            {
                UIColor.Gray.SetColor();
                view.DrawString(row.Text1, new RectangleF(36,  half - halfText, bounds.Width / 2 - 40, Font.LineHeight), Font, UILineBreakMode.TailTruncation);
            }

            row.Image2.Draw(new RectangleF(bounds.Width / 2 + 15, half - 8f, 16f, 16f));

            if (!string.IsNullOrEmpty(row.Text2))
            {
                UIColor.Gray.SetColor();
                view.DrawString(row.Text2, new RectangleF(bounds.Width / 2 + 36,  half - halfText, bounds.Width / 2 - 40, Font.LineHeight), Font, UILineBreakMode.TailTruncation);
            }
        }
    }

}

