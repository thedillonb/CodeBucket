using System;
using CodeBucket.Core.ViewModels.Users;
using System.Linq;
using CodeBucket.Core.Services;
using ReactiveUI;
using CodeBucket.Core.Utils;
using System.Reactive.Linq;
using Splat;
using BitbucketSharp.Models;
using System.Threading.Tasks;

namespace CodeBucket.Core.ViewModels.Groups
{
    public class GroupViewModel : BaseUserCollectionViewModel
	{
        private readonly string _owner, _slug;
        private readonly IApplicationService _applicationService;

        public GroupViewModel(
            GroupModel group,
            IApplicationService applicationService = null)
            : this(group.Owner.Username, group.Slug, group.Name, applicationService)
        {
        }

        public GroupViewModel(string owner, string slug, string name = null,
            IApplicationService applicationService = null)
        {
            _owner = owner;
            _slug = slug;
            _applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = name ?? "Members";
            EmptyMessage = "There are no members.";
        }

        protected override async Task Load(ReactiveList<UserItemViewModel> users)
        {
            var members = await _applicationService.Client.Groups.GetMembers(_owner, _slug);
            var memberUsers = members.Select(x =>
            {
                var username = x.Username;
                var avatar = new Avatar(x.Avatar);
                var displayName = string.Join(" ", x.FirstName, x.LastName);
                var vm = new UserItemViewModel(username, displayName, avatar);
                vm.GoToCommand.Select(__ => new UserViewModel(username)).Subscribe(NavigateTo);
                return vm;
            });

            users.Reset(memberUsers);
        }
	}
}

