using CodeBucket.Core.Services;
using ReactiveUI;
using BitbucketSharp;

namespace CodeBucket.Core.ViewModels.User
{
    public class UserFollowingsViewModel : BaseUserCollectionViewModel, ILoadableViewModel
    {
        public string Name { get; private set; }

        public ReactiveUI.IReactiveCommand LoadCommand { get; }

        public UserFollowingsViewModel(IApplicationService applicationService)
        {
            LoadCommand = ReactiveCommand.CreateAsyncTask(t =>
            {
                Users.Items.Clear();
                return applicationService.Client.ForAllItems(x => x.Users.GetFollowing(Name), Users.Items.AddRange);
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