
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using MonoTouch.CoreGraphics;
using BitbucketBrowser.Utils;

namespace BitbucketBrowser.UI
{
    public partial class IssueCellView : UITableViewCell
    {
        private static UIImage User, Priority, Pencil, Cog;

        private int MessageCount { get; set; }

        static IssueCellView()
        {
            User = new UIImage(Images.Person.CGImage, 1.3f, UIImageOrientation.Up);
            Priority = new UIImage(Images.Priority.CGImage, 1.3f, UIImageOrientation.Up);
            Pencil = new UIImage(Images.Pencil.CGImage, 1.3f, UIImageOrientation.Up);
            Cog = new UIImage(Images.Cog.CGImage, 1.3f, UIImageOrientation.Up);
        }

        public static IssueCellView Create()
        {
            var cell = new IssueCellView();
            var views = NSBundle.MainBundle.LoadNib("IssueCellView", cell, null);
            cell = Runtime.GetNSObject( views.ValueAt(0) ) as IssueCellView;

            cell.AddSubview(new SeperatorIssues() { Frame = new RectangleF(65f, 5f, 1f, cell.Frame.Height - 10f) });

            cell.Image1.Image = Cog;
            cell.Image2.Image = Priority;
            cell.Image3.Image = User;
            cell.Image4.Image = Pencil;


            cell.BackgroundView = new UIImageView(Images.CellGradient);

            //Create the icons
            return cell;
        }

        public IssueCellView() 
            : base ()
        {
        }

        public IssueCellView(IntPtr handle)
            : base(handle)
        {
        }

        public void Bind(IssueModel model)
        {
            var assigned = model.Responsible != null ? model.Responsible.Username : "unassigned";


            Caption.Text = model.Title;
            Label1.Text = model.Status;
            Label2.Text = model.Priority;
            Label3.Text = assigned;
            Label4.Text = DateTime.Parse(model.UtcLastUpdated).ToDaysAgo();
            Number.Text = "#" + model.LocalId;


            if (model.Metadata.Kind.ToLower().Equals("enhancement")) 
                IssueType.Text = "enhance";
            else
                IssueType.Text = model.Metadata.Kind;

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

        static UIFont CountFont = UIFont.BoldSystemFontOfSize (13);



        private class CounterView : UIView
        {
            private int _counter;
            public CounterView(int counter) 
                : base ()
            {
                _counter = counter;
                BackgroundColor = UIColor.Clear;
            }

            public override void Draw(RectangleF rect)
            {
                if (_counter > 0){
                    var ctx = UIGraphics.GetCurrentContext ();
                    var ms = _counter.ToString ();

                    var crect = Bounds;
                    
                    UIColor.Gray.SetFill ();
                    GraphicsUtil.FillRoundedRect (ctx, crect, 3);
                    UIColor.White.SetColor ();
                    crect.X += 5;
                    DrawString (ms, crect, CountFont);
                }
                base.Draw(rect);
            }
        }

        private class SeperatorIssues : UIView
        {
            public SeperatorIssues() 
                : base ()
            {
            }

            public SeperatorIssues(IntPtr handle)
                : base(handle)
            {
            }

            public override void Draw(RectangleF rect)
            {
                base.Draw(rect);

                var context = UIGraphics.GetCurrentContext();
                //context.BeginPath();
                //context.ClipToRect(new RectangleF(63f, 0f, 3f, rect.Height));
                using (var cs = CGColorSpace.CreateDeviceRGB ())
                {
                    using (var gradient = new CGGradient (cs, new float [] { 1f, 1f, 1f, 1.0f, 
                        0.7f, 0.7f, 0.7f, 1f, 
                        1f, 1f, 1.0f, 1.0f }, new float [] {0, 0.5f, 1f}))
                    {
                        context.DrawLinearGradient(gradient, new PointF(0, 0), new PointF(0, rect.Height), 0);
                    }
                }
            }
        }
    }



    public class IssueElement : Element, IElementSizing, IColorizeBackground
    {       
        public string CellReuseIdentifier
        {
            get;set;    
        }

        public UITableViewCellStyle Style
        {
            get;set;    
        }

        public UIColor BackgroundColor { get; set; }

        public IssueModel Model { get; set; }

        public IssueElement(IssueModel m) 
            : base(null)
        {
            this.CellReuseIdentifier = "issueelement";
            this.Style = UITableViewCellStyle.Default;
            Model = m;
        }

        public float GetHeight (UITableView tableView, NSIndexPath indexPath)
        {
            return 69f;
        }

        
        public event NSAction Tapped;


        public override UITableViewCell GetCell (UITableView tv)
        {
            var cell = tv.DequeueReusableCell(this.CellReuseIdentifier) as IssueCellView;

            if (cell == null)
            {
                cell = IssueCellView.Create();

            }

            cell.Bind(Model);

            return cell;
        }



        public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
        {
            base.Selected(dvc, tableView, path);
            if (Tapped != null)
                Tapped();
            tableView.DeselectRow (path, true);
        }

        void IColorizeBackground.WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            //cell.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("/Images/Cells/gradient"));
        }

        public override bool Matches(string text)
        {
            var lowerText = text.ToLower();
            return Model.LocalId.ToString().ToLower().Equals(lowerText) || Model.Title.ToLower().Contains(lowerText);
        }
    }
}

