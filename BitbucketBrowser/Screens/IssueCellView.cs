
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using MonoTouch.Dialog;
using BitbucketSharp.Models;

namespace BitbucketBrowser.UI
{
    public partial class IssueCellView : UITableViewCell
    {
        private static UIImage User, Heart, Fork, Date;

        static IssueCellView()
        {
            User = new UIImage(Images.Followers.CGImage, 1.3f, UIImageOrientation.Up);
            Heart = new UIImage(Images.Heart.CGImage, 1.3f, UIImageOrientation.Up);
            Fork = new UIImage(Images.Fork.CGImage, 1.3f, UIImageOrientation.Up);
            Date = new UIImage(Images.Language.CGImage, 1.3f, UIImageOrientation.Up);
        }

        public static IssueCellView Create()
        {
            var cell = new IssueCellView();
            var views = NSBundle.MainBundle.LoadNib("IssueCellView", cell, null);
            cell = Runtime.GetNSObject( views.ValueAt(0) ) as IssueCellView;

            cell.Image1.Image = User;
            cell.Image2.Image = Heart;
            cell.Image3.Image = Fork;
            cell.Image4.Image = Date;

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

        public void Bind(string caption, string label1, string label2, string label3, string label4)
        {
            Caption.Text = caption;
            Label1.Text = label1;
            Label2.Text = label2;
            Label3.Text = label3;
            Label4.Text = label4;
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

            var assigned = Model.Responsible != null ? Model.Responsible.Username : "Unassigned";
            cell.Bind(Model.Title, assigned, Model.Status, Model.Status, Model.Priority);

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

