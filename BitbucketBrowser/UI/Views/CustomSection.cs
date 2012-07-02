using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Drawing;

namespace BitbucketBrowser.UI
{
    public class CustomSection : Section
    {
        public CustomSection(string title)
            : base(new SectionView(title))
        {
        }

        private class SectionView : UIView
        {
            public string Text { get; set; }
            private static UIFont Font = UIFont.SystemFontOfSize(15f);
            private static UIColor Color = UIColor.FromRGB(0x66, 0x66, 0x66);

            public SectionView(string text)
                : base(new RectangleF(0, 0, 320f, 17f))
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
                Text = text;
            }

            public override void Draw(RectangleF rect)
            {
                base.Draw(rect);
                Color.SetColor();
                DrawString(Text, new System.Drawing.RectangleF(20f, 0, rect.Width - 40f, rect.Height), Font);
            }
        }
    }
}

