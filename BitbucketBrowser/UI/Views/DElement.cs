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
    public class DElement : Element, IElementSizing, IColorizeBackground
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

        public RepositoryDetailedModel Model { get; set; }

        public DElement(RepositoryDetailedModel m) : base(null)
        {
            this.CellReuseIdentifier = "delement";
            this.Style = UITableViewCellStyle.Default;
            Model = m;
        }

        public float GetHeight (UITableView tableView, NSIndexPath indexPath)
        {
            return 66f;
        }

        
        public event NSAction Tapped;


        public override UITableViewCell GetCell (UITableView tv)
        {
            var cell = tv.DequeueReusableCell(this.CellReuseIdentifier) as RepositoryCellView;

            if (cell == null)
            {
                cell = RepositoryCellView.Create();
            }

            cell.Bind(Model.Name, Model.Scm, Model.FollowersCount.ToString(), Model.ForkCount.ToString(), Model.Description);

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
            cell.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("/Images/Cells/gradient"));
        }
    }
}

