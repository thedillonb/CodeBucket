using System;
using CodeBucket.Core.ViewModels.User;
using CodeBucket.Core.Services;
using BitbucketSharp;
using BitbucketSharp.Models.V2;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.Teams
{
    public class TeamMembersViewModel : BaseViewModel, ILoadableViewModel
    {
        public CollectionViewModel<TeamMember> Members { get; } = new CollectionViewModel<TeamMember>();

        public ReactiveCommand<object> GoToMemberCommand { get; } = ReactiveCommand.Create();

        public string Name { get; private set; }

        public IReactiveCommand LoadCommand { get; }

        public TeamMembersViewModel(IApplicationService applicationService)
        {
            GoToMemberCommand.OfType<TeamMember>()
                .Select(x => new ProfileViewModel.NavObject { Username = x.Username })
                .Subscribe(x => ShowViewModel<ProfileViewModel>(x));

            LoadCommand = ReactiveCommand.CreateAsyncTask(_ => {
                Members.Items.Clear();
                return applicationService.Client.ForAllItems(x => x.Teams.GetMembers(Name), Members.Items.AddRange);
            });
        }

        public void Init(NavObject navObject)
        {
            Name = navObject.Name;
        }

        public class NavObject
        {
            public string Name { get; set; }
        }
    }
}

