using CodeFramework.ViewControllers;
using CodeBucket.Core.ViewModels.PullRequests;
using System;
using MonoTouch.Dialog;

namespace CodeBucket.iOS.Views.PullRequests
{
    public class PullRequestCommitsView : ViewModelCollectionDrivenDialogViewController
	{
        public override void ViewDidLoad()
        {
            Title = "Commits".t();
            Root.UnevenRows = true;

            base.ViewDidLoad();

            var vm = (PullRequestCommitsViewModel) ViewModel;
            BindCollection(vm.Commits, x =>
                {
                    var msg = x.Message ?? string.Empty;
                    var firstLine = msg.IndexOf("\n", StringComparison.Ordinal);
                    var desc = firstLine > 0 ? msg.Substring(0, firstLine) : msg;
                    string username;
                    if (x.Author.User != null)
                        username = x.Author.User.DisplayName ?? x.Author.User.Username;
                    else
                    {
                        var bracketStart = x.Author.Raw.IndexOf("<", StringComparison.Ordinal);
                        username = x.Author.Raw.Substring(0, bracketStart > 0 ? bracketStart : x.Author.Raw.Length);
                    }

                    var el = new NameTimeStringElement { Name = username, Time = x.Date.ToDaysAgo(), String = desc, Lines = 4 };
                    el.Tapped += () => vm.GoToChangesetCommand.Execute(x);
                    return el;
                });
        }
	}
}

