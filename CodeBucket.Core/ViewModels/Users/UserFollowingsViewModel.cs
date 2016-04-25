using CodeBucket.Core.Services;
using ReactiveUI;
using BitbucketSharp;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Users
{
    public class UserFollowingsViewModel : BaseUserCollectionViewModel, ILoadableViewModel
    {
        public string Name { get; private set; }

        public IReactiveCommand LoadCommand { get; }

        public UserFollowingsViewModel(IApplicationService applicationService)
        {
            Title = "Following";
            EmptyMessage = "There are no followers.";

            LoadCommand = ReactiveCommand.CreateAsyncTask(t => 
            {
                Users.Clear();
                return applicationService.Client.ForAllItems(x => x.Users.GetFollowing(Name), 
                                                             x => Users.AddRange(x.Select(ToViewModel)));
            });
        }

        public void Init(NavObject navObject)
        {
            Name = navObject.Username;
        }

        public class NavObject
        {
            public string Username { get; set; }
        }
    }
}