using System;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.Commits;
using CodeBucket.Elements;
using CodeBucket.Cells;
using UIKit;
using CodeBucket.Core.Utils;

namespace CodeBucket.Views.Commits
{
	public abstract class BaseCommitsView : ViewModelCollectionDrivenDialogViewController
	{
		public override void ViewDidLoad()
		{
			Title = "Commits";
			Root.UnevenRows = true;

			base.ViewDidLoad();

            TableView.RegisterNibForCellReuse(CommitCellView.Nib, CommitCellView.Key);
            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.EstimatedRowHeight = 80f;

			var vm = (BaseCommitsViewModel) ViewModel;
            BindCollection(vm.Commits, x =>
            {
                var msg = x.Message ?? string.Empty;
                var firstLine = msg.IndexOf("\n", StringComparison.Ordinal);
                var desc = firstLine > 0 ? msg.Substring(0, firstLine) : msg;

                string username;
                if (x?.Author?.User != null)
                {
                    username = x.Author.User.DisplayName ?? x.Author.User.Username;
                }
                else
                {
                    var bracketStart = x.Author.Raw.IndexOf("<", StringComparison.Ordinal);
                    username = x.Author.Raw.Substring(0, bracketStart > 0 ? bracketStart : x.Author.Raw.Length);
                }

                var avatarUrl = x.Author?.User?.Links?.Avatar?.Href;
                var avatar = new Avatar(avatarUrl);
                var el = new CommitElement(username, desc, x.Date, avatar.ToUrl());
                el.Tapped += () => vm.GoToChangesetCommand.Execute(x);
                return el;
            });
		}
	}
}

