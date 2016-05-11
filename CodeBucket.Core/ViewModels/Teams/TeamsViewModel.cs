using System;
using CodeBucket.Core.Services;
using BitbucketSharp.Controllers;
using BitbucketSharp.Models.V2;
using BitbucketSharp;
using ReactiveUI;
using System.Reactive.Linq;
using Splat;
using System.Reactive;

namespace CodeBucket.Core.ViewModels.Teams
{
    public class TeamsViewModel : BaseViewModel, ILoadableViewModel
    {
        public IReadOnlyReactiveList<TeamItemViewModel> Teams { get; }

        public IReactiveCommand<Unit> LoadCommand { get; }

        public TeamsViewModel(IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Teams";

            var teams = new ReactiveList<Team>();
            Teams = teams.CreateDerivedCollection(team =>
            {
                var vm = new TeamItemViewModel(team.Username);
                vm.GoToCommand
                  .Select(x => new TeamViewModel(team))
                  .Subscribe(NavigateTo);
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