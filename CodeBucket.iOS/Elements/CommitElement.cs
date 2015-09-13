using System;
using CodeBucket.Cells;
using Foundation;
using UIKit;
using CodeBucket.ViewControllers;
using Humanizer;

namespace CodeBucket.Elements
{
    public class CommitElement : Element
    {       
        private readonly string _name;
        private readonly string _description;
        private readonly string _time;
        private UIImage _image;
        private readonly string _imageUri;
        public event Action Tapped;

        public UIColor BackgroundColor { get; set; }

        public CommitElement(string name, string description, DateTimeOffset time, string imageUri = null, UIImage image = null)
            : base(null)
        {
            _name = name;
            _description = description;
            _time = time.Humanize();
            //_imageUri = imageUri;
            _image = image ?? Images.Avatar;
        }

        public override UITableViewCell GetCell (UITableView tv)
        {
            var cell = tv.DequeueReusableCell(CommitCellView.Key) as CommitCellView;
            cell.Bind(_name, _description, _time, _image, _imageUri);
            return cell;
        }

        public override bool Matches(string text)
        {
            var lowerText = text.ToLower();
            return _name.ToLower().Contains(lowerText) || _description.ToLower().Contains(lowerText);
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

