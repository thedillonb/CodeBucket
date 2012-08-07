using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.ObjCRuntime;
using BitbucketSharp.Models;


namespace BitbucketBrowser.UI
{
    public class RepositoryElement : Element, IElementSizing, IColorizeBackground
    {       
        private string CellReuseIdentifier { get; set; }

        public UITableViewCellStyle Style { get; set;}

        public UIColor BackgroundColor { get; set; }

        public RepositoryDetailedModel Model { get; set; }

        public bool ShowOwner { get; set; }

        public RepositoryElement(RepositoryDetailedModel m) : base(null)
        {
            this.CellReuseIdentifier = "repositoryelement";
            this.Style = UITableViewCellStyle.Default;
            Model = m;
            ShowOwner = true;
        }

        public float GetHeight (UITableView tableView, NSIndexPath indexPath)
        {
            return 67f;
        }

        
        public event NSAction Tapped;

        public override UITableViewCell GetCell (UITableView tv)
        {
            var cell = tv.DequeueReusableCell(this.CellReuseIdentifier) as RepositoryCellView;

            if (cell == null)
            {
                cell = RepositoryCellView.Create();

            }

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

