using System;
using Foundation;
using UIKit;
using CodeBucket.TableViewCells;
using CodeBucket.Core.Utils;

namespace CodeBucket.DialogElements
{
    public class RepositoryElement : Element
    {       
        private readonly string _name;
        private readonly string _description;
        private readonly string _owner;
        private readonly Avatar _avatar;

        public UIColor BackgroundColor { get; set; }

        public bool ShowOwner { get; set; }

        public RepositoryElement(string name, string description, string owner, Avatar avatar)
        {
            _name = name;
            _description = description;
            _owner = owner;
            _avatar = avatar;
            ShowOwner = true;
        }

        public event Action Tapped;
        
        public override UITableViewCell GetCell (UITableView tv)
        {
            var cell = tv.DequeueReusableCell(RepositoryCellView.Key) as RepositoryCellView ?? RepositoryCellView.Create();
            cell.Bind(_name, _description, ShowOwner ? _owner : null, _avatar);
            return cell;
        }
        
        public override bool Matches(string text)
        {
            var name = _name ?? string.Empty;
            return name.IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1;
        }
        
        public override void Selected(UITableView tableView, NSIndexPath path)
        {
            base.Selected(tableView, path);
            Tapped?.Invoke();
        }
    }
}

