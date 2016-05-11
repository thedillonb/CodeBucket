using System;
using CodeBucket.Core.ViewModels.Users;
using System.Linq;
using CodeBucket.Core.Services;
using ReactiveUI;
using CodeBucket.Core.Utils;
using System.Reactive.Linq;
using System.Reactive;
using Splat;
using BitbucketSharp.Models;

namespace CodeBucket.Core.ViewModels.Groups
{
    public class GroupViewModel : BaseUserCollectionViewModel, ILoadableViewModel
	{
        public IReactiveCommand<Unit> LoadCommand { get; }

        public GroupViewModel(
            GroupModel group,
            IApplicationService applicationService = null)
            : this(group.Owner.Username, group.Slug, group.Name, applicationService)
        {
        }

        public GroupViewModel(string owner, string slug, string name = null,
            IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = name ?? "Members";
            EmptyMessage = "There are no members.";

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var members = await applicationService.Client.Groups.GetMembers(owner, slug);
                var memberUsers = members.Select(x =>
                {
                    var username = x.Username;
                    var avatar = new Avatar(x.Avatar);
                    var displayName = string.Join(" ", x.FirstName, x.LastName);
                    var vm = new UserItemViewModel(username, displayName, avatar);
                    vm.GoToCommand.Select(__ => new UserViewModel(username)).Subscribe(NavigateTo);
                    return vm;
                });

                Users.Reset(memberUsers);
            });
        }
	}
}

