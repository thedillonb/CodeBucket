using System;
using System.Linq;
using ReactiveUI;
using CodeBucket.Core.Services;
using System.Reactive.Linq;
using Splat;
using System.Reactive;
using CodeBucket.Client;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueAssigneeViewModel : ReactiveObject, ILoadableViewModel
    {
        private bool _isLoaded;

        public IReadOnlyReactiveList<IssueAssigneeItemViewModel> Assignees { get; }

        private string _selectedValue;
        public string SelectedValue
        {
            get { return _selectedValue; }
            set { this.RaiseAndSetIfChanged(ref _selectedValue, value); }
        }

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IReactiveCommand<object> DismissCommand { get; } = ReactiveCommand.Create();

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
                if (_isLoaded) return;

                var repo = await applicationService.Client.Repositories.Get(username, repository);

                try
                {
                    if (repo.Owner.Type == "team")
                    {
                        var members = await applicationService.Client.AllItems(x => x.Teams.GetMembers(username));
                        assignees.Reset(members);
                    }
                    else
                    {
                        var privileges = await applicationService.Client.Privileges.GetRepositoryPrivileges(username, repository);
                        assignees.Reset(privileges.Select(x => ConvertUserModel(x.User)));
                    }
                }
                catch
                {
                    assignees.Reset();
                }

                _isLoaded = true;
            });
        }

        private static User ConvertUserModel(Client.V1.User user)
        {
            return new User
            {
                DisplayName = $"{user.FirstName} {user.LastName}",
                Username = user.Username,
                Links = new User.UserLinks
                {
                    Avatar = new Link(user.Avatar)
                }
            };
        }

        private IssueAssigneeItemViewModel CreateItemViewModel(User item)
        {
            var vm = new IssueAssigneeItemViewModel(item.Username, item.Links?.Avatar?.Href, string.Equals(SelectedValue, item.Username));
            vm.SelectCommand.Subscribe(y => SelectedValue = !vm.IsSelected ? vm.Name : null);
            vm.SelectCommand.InvokeCommand(DismissCommand);
            return vm;
        }
    }
}

