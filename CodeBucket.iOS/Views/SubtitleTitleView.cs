using UIKit;

namespace CodeBucket.Views
{
    public class SubtitleTitleView : UIView
    {
        private readonly UILabel _title = new UILabel();
        private readonly UILabel _subtitle = new UILabel();

        public void SetTitles(string title, string subtitle)
        {
            _title.Text = title;
            _subtitle.Text = subtitle;
            //BackgroundColor = UIColor.Red;
        }

        public SubtitleTitleView()
        {
            Frame = new CoreGraphics.CGRect(0f, 0f, 200, 44);

            _title.Frame = new CoreGraphics.CGRect(0, 0, Frame.Width, 28);
            _title.TextColor = UIColor.White;
            _title.TextAlignment = UITextAlignment.Center;
            _title.Font = UIFont.SystemFontOfSize(16);
            _title.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

            _subtitle.Frame = new CoreGraphics.CGRect(0, 28, Frame.Width, 10);
            _subtitle.TextColor = UIColor.White;
            _subtitle.TextAlignment = UITextAlignment.Center;
            _subtitle.Font = UIFont.SystemFontOfSize(11);
            _subtitle.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

            Add(_title);
            Add(_subtitle);
        }
    }
}

