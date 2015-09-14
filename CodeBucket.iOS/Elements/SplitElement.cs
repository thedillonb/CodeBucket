using UIKit;
using CoreGraphics;
using System;
using Foundation;

namespace CodeBucket.Elements
{
    public class SplitElement : CustomElement
    {
        private static readonly UIFont Font = UIFont.SystemFontOfSize(12f);
        private readonly UIFont _font;

        public Row Value { get; set; }

        public SplitElement(Row row)
            : base(UITableViewCellStyle.Default, "splitelement")
        {
            Value = row;
            BackgroundColor = UIColor.White;
            _font = Font.WithSize(Font.PointSize * Element.FontSizeRatio);
        }

        public class Row
        {
            public string Text1, Text2;
            public UIImage Image1, Image2;
        }


        public override nfloat Height(CGRect bounds)
        {
	        return 36f;
        }

		public override UITableViewCell GetCell(UITableView tv)
		{
			var cell = base.GetCell(tv);
			cell.SeparatorInset = UIEdgeInsets.Zero;
            cell.LayoutMargins = UIEdgeInsets.Zero;
            cell.PreservesSuperviewLayoutMargins = false;
			return cell;
		}

        public override void Draw(CGRect bounds, CGContext context, UIView view)
        {
            var width = 0.5f / UIScreen.MainScreen.Scale;
            context.BeginPath();
            context.SetLineWidth(width);
            context.SetStrokeColor(UIColor.FromRGB(150, 150, 154).CGColor);
            var x = bounds.Width / 2f - width;
			context.MoveTo(x, 0f);
			context.AddLineToPoint(x, bounds.Height);
            context.StrokePath();

            var row = Value;
            var half = bounds.Height / 2;
            var halfText = _font.LineHeight / 2 + 1;

            row.Image1.Draw(new CGRect(15, half - 8f, 16f, 16f));

            if (!string.IsNullOrEmpty(row.Text1))
            {
                UIColor.Gray.SetColor();
                new NSString(row.Text1).DrawString(new CGRect(36,  half - halfText, bounds.Width / 2 - 40, _font.LineHeight), _font, UILineBreakMode.TailTruncation);
            }

            row.Image2.Draw(new CGRect(bounds.Width / 2 + 15, half - 8f, 16f, 16f));

            if (!string.IsNullOrEmpty(row.Text2))
            {
                UIColor.Gray.SetColor();
                new NSString(row.Text2).DrawString(new CGRect(bounds.Width / 2 + 36,  half - halfText, bounds.Width / 2 - 40, _font.LineHeight), _font, UILineBreakMode.TailTruncation);
            }
        }
    }

}

