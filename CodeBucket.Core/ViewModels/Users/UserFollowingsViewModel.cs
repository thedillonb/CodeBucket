using CodeBucket.Core.Services;
using ReactiveUI;
using System.Linq;
using Splat;

namespace CodeBucket.Core.ViewModels.Users
{
    public class UserFollowingsViewModel : UsersViewModel
    {
        private readonly string _username;
        private readonly IApplicationService _applicationService;

        public UserFollowingsViewModel(string username, IApplicationService applicationService = null)
        {
            _username = username;
            _applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Following";
            EmptyMessage = "There are no followers.";
        }

        protected override System.Threading.Tasks.Task Load(ReactiveList<UserItemViewModel> users)
        {
            return _applicationService.Client.ForAllItems(x => x.Users.GetFollowing(_username),
                                                          x => users.AddRange(x.Select(ToViewModel)));
        }
    }
}