using System;
using CoreGraphics;
using Foundation;
using UIKit;
using CodeBucket.Core.ViewModels.Issues;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeBucket.TableViewCells
{
    public partial class IssueCellView : BaseTableViewCell<IssueItemViewModel>
    {
        public static readonly UINib Nib = UINib.FromName("IssueCellView", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("IssueCellView");

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

            this.WhenAnyValue(x => x.ViewModel)
                .Where(x => x != null)
                .Subscribe(x =>
                {
                    Caption.Text = x.Title;
                    Label1.Text = x.Status;
                    Label2.Text = x.Priority;
                    Label3.Text = x.Assigned;
                    Label4.Text = x.LastUpdated;
                    Number.Text = "#" + x.Id;
                    IssueType.Text = x.Kind;
                });
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

