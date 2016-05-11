using CodeBucket.Core.Services;
using ReactiveUI;
using BitbucketSharp;
using System.Linq;
using System.Reactive;
using Splat;

namespace CodeBucket.Core.ViewModels.Users
{
    public class TeamFollowingsViewModel : BaseUserCollectionViewModel, ILoadableViewModel
    {
        public IReactiveCommand<Unit> LoadCommand { get; }

        public TeamFollowingsViewModel(string name, IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Following";
            EmptyMessage = "There are no followers.";

            LoadCommand = ReactiveCommand.CreateAsyncTask(t =>
            {
                Users.Clear();
                return applicationService.Client
                    .ForAllItems(x => x.Teams.GetFollowing(name),
                                 x => Users.AddRange(x.Select(ToViewModel)));
            });
        }
    }
}