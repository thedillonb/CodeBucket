using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace BitbucketBrowser.UI
{
    public class ErrorView : UIView
    {
        private static UIImage Alert = UIImage.FromBundle("/Images/warning.png");

        private static UIFont TitleFont = UIFont.SystemFontOfSize(15f); 

        public string Title { get; set; }
        public string Detail { get; set; }

        private ErrorView()
        {
        }

        public static ErrorView Show(UIView parent, string title, string error = "")
        {
            var ror = new ErrorView() { Title = title, Detail = error, Frame = parent.Bounds, BackgroundColor = UIColor.White };
            parent.AddSubview(ror);
            ror.SetNeedsDisplay();
            return ror;
        }

        public override void Draw(System.Drawing.RectangleF rect)
        {
            base.Draw(rect);
            Alert.Draw(new System.Drawing.RectangleF(rect.Width / 2 - Alert.Size.Width / 2,
                                         rect.Height / 2 - Alert.Size.Height - 2f,
                                         Alert.Size.Width,
                                         Alert.Size.Height));

            var ty = rect.Height / 2 + 2f;
            DrawString(Title, new RectangleF(0, ty, rect.Width, TitleFont.LineHeight * 3), TitleFont, UILineBreakMode.WordWrap, UITextAlignment.Center);
        }
    }

    public class Alert 
    {
        public static void Show(string title, string message)
        {
            var alert = new UIAlertView();
            alert.Title = title;
            alert.Message = message;
            alert.DismissWithClickedButtonIndex(alert.AddButton("Ok"), true);
            alert.Show();
        }
    }

}

