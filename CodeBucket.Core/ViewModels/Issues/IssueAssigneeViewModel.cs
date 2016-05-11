using System;
using System.Linq;
using ReactiveUI;
using CodeBucket.Core.Services;
using BitbucketSharp;
using System.Reactive.Linq;
using BitbucketSharp.Models.V2;
using Splat;
using System.Reactive;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueAssigneeViewModel : ReactiveObject, ILoadableViewModel
    {
        public IReadOnlyReactiveList<IssueAssigneeItemViewModel> Assignees { get; }

        private string _selectedValue;
        public string SelectedValue
        {
            get { return _selectedValue; }
            private set { this.RaiseAndSetIfChanged(ref _selectedValue, value); }
        }

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IssueAssigneeViewModel(
            string username, string repository,
            IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            var assignees = new ReactiveList<User>();
            Assignees = assignees.CreateDerivedCollection(CreateItemViewModel);

            this.WhenAnyValue(x => x.SelectedValue)
                .SelectMany(_ => Assignees)
                .Subscribe(x => x.IsSelected = string.Equals(x.Name, SelectedValue));
            
            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var repo = await applicationService.Client.Repositories.GetRepository(username, repository);

                try
                {
                    if (repo.Owner.Type == "team")
                    {
                        var members = await applicationService.Client.AllItems(x => x.Teams.GetMembers(username));
                        assignees.Reset(members);
                    }
                    else
                    {
                        var privileges = await applicationService.Client.Repositories.GetPrivileges(username, repository);
                        assignees.Reset(privileges.Select(x => ConvertUserModel(x.User)));
                    }
                }
                catch
                {
                    assignees.Reset();
                }
            });
        }

        private static User ConvertUserModel(BitbucketSharp.Models.UserModel user)
        {
            return new User
            {
                DisplayName = $"{user.FirstName} {user.LastName}",
                Username = user.Username,
                Links = new User.LinksModel
                {
                    Avatar = new LinkModel
                    {
                        Href = user.Avatar
                    }
                }
            };
        }

        private IssueAssigneeItemViewModel CreateItemViewModel(User item)
        {
            var vm = new IssueAssigneeItemViewModel(item.Username, item.Links?.Avatar?.Href, string.Equals(SelectedValue, item.Username));
            vm.WhenAnyValue(y => y.IsSelected).Skip(1).Subscribe(y => SelectedValue = y ? item.Username : null);
            return vm;
        }
    }
}

