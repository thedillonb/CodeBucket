using CodeBucket.Core.Services;
using ReactiveUI;
using BitbucketSharp;
using System.Linq;
using CodeBucket.Core.ViewModels.Users;
using Splat;
using System.Reactive;

namespace CodeBucket.Core.ViewModels.Teams
{
    public class TeamFollowersViewModel : BaseUserCollectionViewModel, ILoadableViewModel
    {
        public IReactiveCommand<Unit> LoadCommand { get; }

        public TeamFollowersViewModel(string name, IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Followers";
            EmptyMessage = "There are no followers.";

            LoadCommand = ReactiveCommand.CreateAsyncTask(t =>
            {
                Users.Clear();
                return applicationService.Client
                    .ForAllItems(x => x.Teams.GetFollowers(name),
                                 x => Users.AddRange(x.Select(ToViewModel)));
            });
        }
    }
}

