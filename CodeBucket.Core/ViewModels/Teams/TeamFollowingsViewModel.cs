using CodeBucket.Core.Services;
using ReactiveUI;
using BitbucketSharp;
using System.Linq;
using Splat;

namespace CodeBucket.Core.ViewModels.Users
{
    public class TeamFollowingsViewModel : BaseUserCollectionViewModel
    {
        private readonly string _name;
        private readonly IApplicationService _applicationService;

        public TeamFollowingsViewModel(string name, IApplicationService applicationService = null)
        {
            _name = name;
            _applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Following";
            EmptyMessage = "There are no followers.";
        }

        protected override System.Threading.Tasks.Task Load(ReactiveList<UserItemViewModel> users)
        {
            return _applicationService.Client
                    .ForAllItems(x => x.Teams.GetFollowing(_name),
                                 x => users.AddRange(x.Select(ToViewModel)));
        }
    }
}