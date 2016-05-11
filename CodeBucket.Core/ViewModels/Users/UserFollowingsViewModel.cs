using CodeBucket.Core.Services;
using ReactiveUI;
using BitbucketSharp;
using System.Linq;
using Splat;
using System.Reactive;

namespace CodeBucket.Core.ViewModels.Users
{
    public class UserFollowingsViewModel : BaseUserCollectionViewModel, ILoadableViewModel
    {
        public string Name { get; private set; }

        public IReactiveCommand<Unit> LoadCommand { get; }

        public UserFollowingsViewModel(string username, IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Following";
            EmptyMessage = "There are no followers.";

            LoadCommand = ReactiveCommand.CreateAsyncTask(t => 
            {
                Users.Clear();
                return applicationService.Client.ForAllItems(x => x.Users.GetFollowing(username), 
                                                             x => Users.AddRange(x.Select(ToViewModel)));
            });
        }
    }
}