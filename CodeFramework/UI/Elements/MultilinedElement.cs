using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace CodeFramework.UI.Elements
{
    public class MultilinedElement : CustomElement
    {
        private static float Padding = 12f;
        private static float PaddingX = 8f;

        public string Value { get; set; }
        public UIFont CaptionFont { get; set; }
        public UIFont ValueFont { get; set; }
        public UIColor CaptionColor { get; set; }
        public UIColor ValueColor { get; set; }

        public MultilinedElement(string caption)
            : base(UITableViewCellStyle.Default, "multilinedelement")
        {
            this.Caption = caption;
            BackgroundColor = UIColor.FromRGB(247, 247, 247);
            CaptionFont = UIFont.BoldSystemFontOfSize(15f);
            ValueFont = UIFont.SystemFontOfSize(13f);
            CaptionColor = ValueColor = UIColor.FromRGB(41, 41, 41);
        }

        public override void Draw(RectangleF bounds, MonoTouch.CoreGraphics.CGContext context, UIView view)
        {
            CaptionColor.SetColor();
            view.DrawString(Caption, new RectangleF(PaddingX, Padding, bounds.Width - Padding * 2, bounds.Height - Padding * 2), CaptionFont);

            if (Value != null)
            {
                ValueColor.SetColor();
                view.DrawString(Value, new RectangleF(PaddingX, Padding + CaptionFont.LineHeight + 6f, bounds.Width - Padding * 2, bounds.Height), ValueFont, UILineBreakMode.WordWrap);
            }
        }


        public override float Height(System.Drawing.RectangleF bounds)
        {
            var width = bounds.Width;
            if (IsTappedAssigned)
                width -= 20f;
             
            var textHeight = Caption.MonoStringHeight(CaptionFont, width - PaddingX * 2);

            if (Value != null)
            {
                textHeight += 6f;
                textHeight += Value.MonoStringHeight(ValueFont, width - PaddingX * 2);
            }

            return textHeight + Padding * 2;
        }
    }
}

