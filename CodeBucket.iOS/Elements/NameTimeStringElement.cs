using System;
using UIKit;
using Humanizer;
using CodeBucket.Cells;
using CodeBucket.ViewControllers;
using Foundation;

namespace CodeBucket.Elements
{
    public class NameTimeStringElement : Element
    {       
        private readonly string _name;
        private readonly string _description;
        private readonly string _time;
        private UIImage _image;
        private readonly string _imageUri;
        public event Action Tapped;
        private readonly nint _lines;

        public NameTimeStringElement(string name, string description, DateTimeOffset time, string imageUri = null, UIImage image = null, int lines = 0)
            : base(null)
        {
            _name = name;
            _description = description;
            _time = time.Humanize();
            _imageUri = imageUri;
            _image = image ?? Images.Avatar;
            _lines = (nint)lines;
        }

        public override UITableViewCell GetCell (UITableView tv)
        {
            var cell = tv.DequeueReusableCell(CommitCellView.Key) as CommitCellView ?? CommitCellView.Create();
            cell.MaxContentLines = _lines;
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

