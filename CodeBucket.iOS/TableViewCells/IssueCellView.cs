using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;
using Humanizer;

namespace CodeBucket.TableViewCells
{
    public partial class IssueCellView : UITableViewCell
    {
        public static readonly NSString Key = new NSString("IssueCellView");

        public static IssueCellView Create()
        {
            var views = NSBundle.MainBundle.LoadNib("IssueCellView", null, null);
            return Runtime.GetNSObject<IssueCellView>(views.ValueAt(0));
        }

        public IssueCellView()
        {
        }

        public IssueCellView(IntPtr handle)
            : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            Caption.TextColor = Theme.CurrentTheme.MainTitleColor;
            Number.TextColor = Theme.CurrentTheme.MainTitleColor;
            AddSubview(new SeperatorIssues {Frame = new CGRect(65f, 5f, 1f, Frame.Height - 10f)});
            Image1.Image = Theme.CurrentTheme.IssueCellImage1;
            Image2.Image = Theme.CurrentTheme.IssueCellImage2;
            Image3.Image = Theme.CurrentTheme.IssueCellImage3;
            Image4.Image = Theme.CurrentTheme.IssueCellImage4;
            SeparatorInset = new UIEdgeInsets(0, 0, 0, 0);
        }

        public void Bind(string title, string status, string priority, string assigned, DateTimeOffset lastUpdated, string id, string kind)
        {
            Caption.Text = title;
            Label1.Text = status;
            Label2.Text = priority;
            Label3.Text = assigned;
            Label4.Text = lastUpdated.Humanize();
            Number.Text = "#" + id;
            IssueType.Text = kind;

            /*
            if (model.CommentCount > 0)
            {
                var ms = model.CommentCount.ToString ();
                var ssize = ms.MonoStringLength(CountFont);
                var boxWidth = Math.Min (22 + ssize, 18);
                AddSubview(new CounterView(model.CommentCount) { Frame = new RectangleF(Bounds.Width-30-boxWidth, Bounds.Height / 2 - 8, boxWidth, 16) });
            }
            */
        }

        private class SeperatorIssues : UIView
        {
            public SeperatorIssues()
            {
            }

            public SeperatorIssues(IntPtr handle)
                : base(handle)
            {
            }

            public override void Draw(CGRect rect)
            {
                base.Draw(rect);

                var context = UIGraphics.GetCurrentContext();
                //context.BeginPath();
                //context.ClipToRect(new RectangleF(63f, 0f, 3f, rect.Height));
                using (var cs = CGColorSpace.CreateDeviceRGB ())
                {
                    using (var gradient = new CGGradient (cs, new nfloat[] { 1f, 1f, 1f, 1.0f, 
                        0.7f, 0.7f, 0.7f, 1f, 
                        1f, 1f, 1.0f, 1.0f }, new nfloat[] {0, 0.5f, 1f}))
                    {
                        context.DrawLinearGradient(gradient, new CGPoint(0, 0), new CGPoint(0, rect.Height), 0);
                    }
                }
            }
        }
    }
}

