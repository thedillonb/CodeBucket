using System;
using Foundation;
using UIKit;
using CodeBucket.Client.Models;
using CodeBucket.Core.Utils;
using CodeBucket.TableViewCells;
using Humanizer;

namespace CodeBucket.DialogElements
{
    public class CommitElement : Element
    {   
        private readonly Avatar _avatar;
        private readonly string _name;
        private readonly string _description;
        private readonly string _date;

        public event Action Clicked;

        public CommitElement(string name, string description, DateTimeOffset date, Avatar avatar)
        {
            _name = name;
            _description = description;
            _date = date.Humanize();
            _avatar = avatar;
        }

        public override UITableViewCell GetCell (UITableView tv)
        {
            var c = tv.DequeueReusableCell(CommitCellView.Key) as CommitCellView ?? CommitCellView.Create();
            c.Bind(_name, _description, _date, _avatar);
            return c;
        }

        public override bool Matches(string text)
        {
                return _description?.ToLower().Contains(text.ToLower()) ?? false;
        }

        public override void Selected(UITableView tableView, NSIndexPath path)
        {
            base.Selected(tableView, path);
            Clicked?.Invoke();
        }
    }
}



