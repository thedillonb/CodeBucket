using System;
using Foundation;
using UIKit;
using CodeBucket.TableViewCells;
using BitbucketSharp.Models;
using Humanizer;
using CodeBucket.Core.Utils;

namespace CodeBucket.DialogElements
{
    public class PullRequestElement : Element
    {   
        private readonly Action _action;    
        private readonly PullRequestModel _model;
        private readonly Avatar _avatar;

        public PullRequestElement(PullRequestModel model, Action action)
        {
            _model = model;
            _action = action;
            _avatar = new Avatar(_model.Author?.Links?.Avatar?.Href);
        }

        public override UITableViewCell GetCell (UITableView tv)
        {
            var c = tv.DequeueReusableCell(PullRequestCellView.Key) as PullRequestCellView ?? PullRequestCellView.Create();
            c.Bind(_model.Title, _model.CreatedOn.Humanize(), _avatar);
            return c;
        }

        public override bool Matches(string text)
        {
            return _model.Title.ToLower().Contains(text.ToLower());
        }

        public override void Selected(UITableView tableView, NSIndexPath path)
        {
            base.Selected(tableView, path);
            _action?.Invoke();
        }
    }
}

