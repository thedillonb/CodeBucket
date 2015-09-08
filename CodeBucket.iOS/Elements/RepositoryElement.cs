using System;
using CodeBucket.Cells;
using Foundation;
using UIKit;
using CodeBucket.ViewControllers;

namespace CodeBucket.Elements
{
    public class RepositoryElement : Element
    {       
        private readonly string _name;
        private readonly string _description;
        private readonly string _owner;
        private UIImage _image;
        private readonly Uri _imageUri;
        public event Action Tapped;

        public UIColor BackgroundColor { get; set; }

        public RepositoryElement(string name, string description, string owner, Uri imageUri = null, UIImage image = null)
            : base(null)
        {
            _name = name;
            _description = description;
            _owner = owner;
            _imageUri = imageUri;
            _image = image;
        }
        
        public override UITableViewCell GetCell (UITableView tv)
        {
            var cell = tv.DequeueReusableCell(RepositoryCellView.Key) as RepositoryCellView;
            cell.Bind(_name, _description, _owner, _image, _imageUri);
            return cell;
        }
        
        public override bool Matches(string text)
        {
            return _name.ToLower().Contains(text.ToLower());
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

