
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using BitbucketBrowser.UI;
using CodeFramework.UI.Views;
using MonoTouch.Dialog;
using BitbucketSharp.Models;

namespace BitbucketBrowser
{
    public partial class RepositoryCellView : UITableViewCell
    {
        private static UIImage Commit;
        private static UIImage Heart;
        private static UIImage Fork;

        static RepositoryCellView()
        {
            Commit = new UIImage(Images.ScmType.CGImage, 1.3f, UIImageOrientation.Up);
            Heart = new UIImage(Images.Heart.CGImage, 1.3f, UIImageOrientation.Up);
            Fork = new UIImage(Images.Fork.CGImage, 1.3f, UIImageOrientation.Up);
        }

        public static RepositoryCellView Create()
        {
            var cell = new RepositoryCellView();
            var views = NSBundle.MainBundle.LoadNib("RepositoryCellView", cell, null);
            cell = Runtime.GetNSObject( views.ValueAt(0) ) as RepositoryCellView;

            cell.Image1.Image = Commit;
            cell.Image2.Image = Heart;
            cell.Image3.Image = Fork;

            cell.BackgroundView = new CellBackgroundView();

            //Create the icons
            return cell;
        }


        public RepositoryCellView()
            : base()
        {
        }

        public RepositoryCellView(IntPtr handle)
            : base(handle)
        {
        }

        public void Bind(string name, string name1, string name2, string name3, string description, string repoOwner)
        {
            Caption.Text = name;
            Label1.Text = name1;
            Label2.Text = name2;
            Label3.Text = name3;
            Description.Text = description ?? "";

            RepoName.Frame = new RectangleF(8, 6, 71, 21);
            Caption.Frame = new RectangleF(72, 4, 179, 21);

            RepoName.Text = repoOwner != null ? repoOwner + "/" : string.Empty;
            RepoName.SizeToFit();

            Caption.Frame = new RectangleF(RepoName.Frame.Right, Caption.Frame.Y, this.Description.Frame.Width - RepoName.Frame.Right + RepoName.Frame.Left, Caption.Frame.Height);

            if (string.IsNullOrEmpty(Description.Text))
            {
                Caption.Frame = new RectangleF(Caption.Frame.X, this.Frame.Height / 2 - Caption.Font.LineHeight / 2 - 2, Caption.Frame.Width, Caption.Frame.Height);
                RepoName.Frame = new RectangleF(RepoName.Frame.X, this.Frame.Height / 2 - RepoName.Font.LineHeight / 2, RepoName.Frame.Width, RepoName.Frame.Height);
            }
            else
            {
                var height = Description.Text.MonoStringHeight(Description.Font, Description.Frame.Width);
                if (height < Description.Font.LineHeight + 3)
                {
                    Caption.Frame = new RectangleF(Caption.Frame.X, Caption.Frame.Y + 8f, Caption.Frame.Width, Caption.Frame.Height);
                    RepoName.Frame = new RectangleF(RepoName.Frame.X, RepoName.Frame.Y + 8f, RepoName.Frame.Width, RepoName.Frame.Height);
                }
            }
        }
    }

    public class RepositoryElement : Element, IElementSizing, IColorizeBackground
    {       
        public UITableViewCellStyle Style { get; set;}

        public UIColor BackgroundColor { get; set; }

        public RepositoryDetailedModel Model { get; set; }

        public bool ShowOwner { get; set; }

        public RepositoryElement(RepositoryDetailedModel m) : base(null)
        {
            this.Style = UITableViewCellStyle.Default;
            Model = m;
            ShowOwner = true;
        }

        public float GetHeight (UITableView tableView, NSIndexPath indexPath)
        {
            return 67f;
        }

        protected override NSString CellKey {
            get {
                return new NSString("RepositoryCellView");
            }
        }

        
        public event NSAction Tapped;

        public override UITableViewCell GetCell (UITableView tv)
        {
            var cell = tv.DequeueReusableCell(CellKey) as RepositoryCellView;
            if (cell == null)
                cell = RepositoryCellView.Create();
            return cell;
        }

        public override bool Matches(string text)
        {
            return Model.Name.ToLower().Contains(text.ToLower());
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
            var c = cell as RepositoryCellView;
            if (c != null)
                c.Bind(Model.Name, Model.Scm, Model.FollowersCount.ToString(), Model.ForkCount.ToString(), Model.Description, ShowOwner ? Model.Owner : null);
        }
    }
}

