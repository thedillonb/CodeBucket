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
        public CollectionViewModel<Team> Teams { get; } = new CollectionViewModel<Team>();

        public ReactiveCommand<object> GoToTeamCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand LoadCommand { get; }

        public TeamsViewModel(IApplicationService applicationService)
        {
            GoToTeamCommand.OfType<Team>()
                .Select(x => new TeamViewModel.NavObject { Name = x.Username })
                .Subscribe(x => ShowViewModel<TeamViewModel>(x));

            LoadCommand = ReactiveCommand.CreateAsyncTask(_ => {
                Teams.Items.Clear();
                return applicationService.Client.ForAllItems(x => x.Teams.GetTeams(TeamRole.Member), Teams.Items.AddRange);
            });
        }
    }
}