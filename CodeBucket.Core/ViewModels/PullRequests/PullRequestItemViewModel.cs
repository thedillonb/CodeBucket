using System.Reactive;
using CodeBucket.Core.Utils;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.PullRequests
{
    public class PullRequestItemViewModel : ReactiveObject, ICanGoToViewModel
    {
        public string Title { get; }

        public Avatar Avatar { get; }

        public string CreatedOn { get; }

        public ReactiveCommand<Unit, Unit> GoToCommand { get; } = ReactiveCommandFactory.Empty();

        public PullRequestItemViewModel(string title, Avatar avatar, string createdOn)
        {
            Title = title;
            Avatar = avatar;
            CreatedOn = createdOn;
        }
    }
}

