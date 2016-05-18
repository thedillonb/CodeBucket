using CodeBucket.Core.ViewModels.Users;
using CodeBucket.Core.Services;
using BitbucketSharp;
using ReactiveUI;
using System.Reactive.Linq;
using System.Linq;
using Splat;

namespace CodeBucket.Core.ViewModels.Teams
{
    public class TeamMembersViewModel : BaseUserCollectionViewModel
    {
        private readonly string _name;
        private readonly IApplicationService _applicationService;

        public TeamMembersViewModel(string name, IApplicationService applicationService = null)
        {
            _name = name;
            _applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Members";
            EmptyMessage = "There are no members.";
        }

        protected override System.Threading.Tasks.Task Load(ReactiveList<UserItemViewModel> users)
        {
            return _applicationService.Client.ForAllItems(x => x.Teams.GetMembers(_name),
                                                          x => users.AddRange(x.Select(ToViewModel)));
        }
    }
}

