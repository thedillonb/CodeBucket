using System;
using UIKit;
using Humanizer;
using CodeBucket.Cells;
using CodeBucket.ViewControllers;
using Foundation;
using CodeBucket.Core.Utils;

namespace CodeBucket.Elements
{
    public class PullRequestElement : Element
    {       
        private readonly Avatar _avatar;
        private readonly string _name;
        private readonly string _time;
        private UIImage _image;
        public event Action Tapped;

        public PullRequestElement(string name, DateTimeOffset time, Avatar avatar, UIImage image = null)
            : base(null)
        {
            _name = name;
            _time = time.Humanize();
            _avatar = avatar;
            _image = image ?? Images.Avatar;
        }

        public override UITableViewCell GetCell (UITableView tv)
        {
            var cell = tv.DequeueReusableCell(PullRequestCellView.Key) as PullRequestCellView;
            cell.Bind(_name, _time, _avatar, _image);
            return cell;
        }

        public override bool Matches(string text)
        {
            var lowerText = text.ToLower();
            return _name.ToLower().Contains(lowerText);
        }

        public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
        {
            base.Selected(dvc, tableView, path);
            if (Tapped != null)
                Tapped();
            tableView.DeselectRow (path, true);
        }
    }
}

