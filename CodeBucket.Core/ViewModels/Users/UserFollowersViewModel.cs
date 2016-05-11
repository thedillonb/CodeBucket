using CodeBucket.Core.Services;
using ReactiveUI;
using BitbucketSharp;
using System.Linq;
using System.Reactive;
using Splat;

namespace CodeBucket.Core.ViewModels.Users
{
    public class UserFollowersViewModel : BaseUserCollectionViewModel, ILoadableViewModel
    {
        public IReactiveCommand<Unit> LoadCommand { get; }

        public UserFollowersViewModel(string username, IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Followers";
            EmptyMessage = "There are no followers.";
            
            LoadCommand = ReactiveCommand.CreateAsyncTask(t => 
            {
                Users.Clear();
                return applicationService.Client
                    .ForAllItems(x => x.Users.GetFollowers(username),
                                 x => Users.AddRange(x.Select(ToViewModel)));
            });
        }
    }
}

