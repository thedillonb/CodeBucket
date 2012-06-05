using MonoTouch.UIKit;
using System.Drawing;

namespace BitbucketBrowser.UI
{
    public class HeaderView : UIView
    {
        private static float XPad = 14f;
        private static float YPad = 10f;
        private static readonly UIFont TitleFont = UIFont.BoldSystemFontOfSize(18);
        private static readonly UIFont SubtitleFont = UIFont.SystemFontOfSize(13);

        public string Title { get; set; }

        public string Subtitle { get; set; }
        
        public HeaderView(float width)
            : base(new RectangleF(0, 0, width, 60f))
        {
            Layer.MasksToBounds = false;
            Layer.ShadowColor = UIColor.DarkGray.CGColor;
            Layer.ShadowOpacity = 1.0f;
            Layer.ShadowOffset = new SizeF(0, 3f);
            BackgroundColor = UIColor.FromRGB(0.94f, 0.94f, 0.94f);
        }

        public override void Draw(RectangleF rect)
        {
            base.Draw(rect);
            float titleY = string.IsNullOrWhiteSpace(Subtitle) ? rect.Height / 2 - TitleFont.LineHeight / 2 : YPad;

            DrawString(
                    Title,
                    new RectangleF(XPad, titleY, rect.Width - XPad * 2, TitleFont.LineHeight),
                    TitleFont,
                    UILineBreakMode.TailTruncation
            );

            if (!string.IsNullOrWhiteSpace(Subtitle))
            {
                UIColor.FromRGB(0.3f, 0.3f, 0.3f).SetColor();

                DrawString(
                    Subtitle,
                    new RectangleF(XPad, YPad + TitleFont.LineHeight + 2f, rect.Width - XPad * 2, SubtitleFont.LineHeight),
                    SubtitleFont,
                    UILineBreakMode.TailTruncation
                );
            }
        }
    }
}

