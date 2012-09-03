using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace CodeFramework.UI.Views
{
    public class ErrorView : UIView
    {
        public static UIImage AlertImage;
        public static UIFont TitleFont = UIFont.SystemFontOfSize(15f); 

        public string Title { get; set; }
        public string Detail { get; set; }

        private ErrorView() { }

        public static ErrorView Show(UIView parent, string title, string error = "")
        {
            if (parent == null)
                return null;

            var ror = new ErrorView() { Title = title, Detail = error, Frame = parent.Bounds, BackgroundColor = UIColor.White };
            parent.AddSubview(ror);
            ror.SetNeedsDisplay();
            return ror;
        }

        public override void Draw(System.Drawing.RectangleF rect)
        {
            base.Draw(rect);
            var img = AlertImage;
            img.Draw(new System.Drawing.RectangleF(rect.Width / 2 - img.Size.Width / 2,
                                         rect.Height / 2 - img.Size.Height - 2f,
                                         img.Size.Width,
                                         img.Size.Height));

            var ty = rect.Height / 2 + 2f;
            DrawString(Title, new RectangleF(0, ty, rect.Width, TitleFont.LineHeight * 3), TitleFont, UILineBreakMode.WordWrap, UITextAlignment.Center);
        }
    }
}

