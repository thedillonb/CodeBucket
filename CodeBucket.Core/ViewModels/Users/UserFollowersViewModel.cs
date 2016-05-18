using CodeBucket.Core.Services;
using ReactiveUI;
using BitbucketSharp;
using System.Linq;
using Splat;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.Users
{
    public class UserFollowersViewModel : BaseUserCollectionViewModel
    {
        private readonly string _username;
        private readonly IApplicationService _applicationService;

        public UserFollowersViewModel(string username, IApplicationService applicationService = null)
        {
            _username = username;
            _applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Followers";
            EmptyMessage = "There are no followers.";
        }

        protected override System.Threading.Tasks.Task Load(ReactiveList<UserItemViewModel> users)
        {
            return _applicationService.Client
                .ForAllItems(x => x.Users.GetFollowers(_username),
                             x => users.AddRange(x.Select(ToViewModel)));
        }
    }
}

