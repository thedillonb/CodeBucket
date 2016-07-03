using CodeBucket.Core.Utils;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.PullRequests
{
    public class PullRequestItemViewModel : ReactiveObject, ICanGoToViewModel
    {
        public string Title { get; }

        public Avatar Avatar { get; }

        public string CreatedOn { get; }

        public IReactiveCommand<object> GoToCommand { get; } = ReactiveCommand.Create();

        public PullRequestItemViewModel(string title, Avatar avatar, string createdOn)
        {
            Title = title;
            Avatar = avatar;
            CreatedOn = createdOn;
        }
    }
}
