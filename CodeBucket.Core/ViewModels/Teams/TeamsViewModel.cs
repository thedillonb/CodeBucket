using System;
using CodeBucket.Core.Services;
using BitbucketSharp.Controllers;
using BitbucketSharp.Models.V2;
using BitbucketSharp;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.Teams
{
    public class TeamsViewModel : BaseViewModel, ILoadableViewModel
    {
        public IReadOnlyReactiveList<TeamItemViewModel> Teams { get; }

        public IReactiveCommand LoadCommand { get; }

        public TeamsViewModel(IApplicationService applicationService)
        {
            var teams = new ReactiveList<Team>();
            Teams = teams.CreateDerivedCollection(team =>
            {
                var username = team.Username;
                var vm = new TeamItemViewModel(username);
                vm.GoToCommand
                  .Select(x => new TeamViewModel.NavObject { Name = username })
                  .Subscribe(x => ShowViewModel<TeamViewModel>(x));
                return vm;
            });

            LoadCommand = ReactiveCommand.CreateAsyncTask(_ =>
            {
                teams.Reset();
                return applicationService.Client.ForAllItems(x => x.Teams.GetTeams(TeamRole.Member), teams.AddRange);
            });
        }
    }
}