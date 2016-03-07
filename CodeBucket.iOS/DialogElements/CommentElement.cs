using UIKit;
using System;
using CodeBucket.TableViewCells;
using CodeBucket.Core.Utils;
using Humanizer;

namespace CodeBucket.DialogElements
{
    public class CommentElement : Element
    {   
        private readonly string _title, _message, _date;
        private readonly Avatar _avatar;

        public CommentElement(string title, string message, DateTimeOffset date, Avatar avatar)
        {
            _title = title;
            _message = message;
            _date = date.Humanize();
            _avatar = avatar;
        }

        public override UITableViewCell GetCell (UITableView tv)
        {
            var c = tv.DequeueReusableCell(CommitCellView.Key) as CommitCellView ?? CommitCellView.Create();
            c.Bind(_title, _message, _date, _avatar);
            return c;
        }

        public override bool Matches(string text)
        {
            return _message?.ToLower().Contains(text.ToLower()) ?? false;
        }
    }
}

