using CodeBucket.Core.Services;
using ReactiveUI;
using System.Linq;
using CodeBucket.Core.ViewModels.Users;
using Splat;

namespace CodeBucket.Core.ViewModels.Teams
{
    public class TeamFollowersViewModel : UsersViewModel
    {
        private readonly string _name;
        private readonly IApplicationService _applicationService;

        public TeamFollowersViewModel(string name, IApplicationService applicationService = null)
        {
            _name = name;
            _applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Followers";
            EmptyMessage = "There are no followers.";
        }

        protected override System.Threading.Tasks.Task Load(ReactiveList<UserItemViewModel> users)
        {
            return _applicationService.Client
                    .ForAllItems(x => x.Teams.GetFollowers(_name),
                                 x => users.AddRange(x.Select(ToViewModel)));
        }
    }
}

