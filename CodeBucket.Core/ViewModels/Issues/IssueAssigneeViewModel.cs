using System;
using System.Linq;
using ReactiveUI;
using CodeBucket.Core.Services;
using BitbucketSharp;
using System.Reactive.Linq;
using BitbucketSharp.Models.V2;

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

        public string Username { get; private set; }

        public string Repository { get; private set; }

        public IReactiveCommand LoadCommand { get; }

        public void Init(string username, string repository) 
		{
			Username = username;
            Repository = repository;
		}

        public IssueAssigneeViewModel(IApplicationService applicationService)
        {
            var assignees = new ReactiveList<User>();
            Assignees = assignees.CreateDerivedCollection(CreateItemViewModel);

            this.WhenAnyValue(x => x.SelectedValue)
                .SelectMany(_ => Assignees)
                .Subscribe(x => x.IsSelected = string.Equals(x.Name, SelectedValue));
            
            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var repository = await applicationService.Client.Repositories.GetRepository(Username, Repository);

                try
                {
                    if (repository.Owner.Type == "team")
                    {
                        var members = await applicationService.Client.AllItems(x => x.Teams.GetMembers(Username));
                        assignees.Reset(members);
                    }
                    else
                    {
                        var privileges = await applicationService.Client.Repositories.GetPrivileges(Username, Repository);
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

