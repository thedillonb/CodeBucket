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
    public class TeamsViewModel : BaseViewModel, ILoadableViewModel, IProvidesSearch, IListViewModel<TeamItemViewModel>
    {
        public IReadOnlyReactiveList<TeamItemViewModel> Items { get; }

        public IReactiveCommand<Unit> LoadCommand { get; }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

        public TeamsViewModel(IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Teams";

            var teams = new ReactiveList<Team>();
            Items = teams.CreateDerivedCollection(team =>
            {
                var vm = new TeamItemViewModel(team.Username);
                vm.GoToCommand
                  .Select(x => new TeamViewModel(team))
                  .Subscribe(NavigateTo);
                return vm;
            }, 
            x => x.Username.ContainsKeyword(SearchText), 
            signalReset: this.WhenAnyValue(x => x.SearchText));

            LoadCommand = ReactiveCommand.CreateAsyncTask(_ =>
            {
                teams.Clear();
                return applicationService.Client.ForAllItems(x => x.Teams.GetTeams(TeamRole.Member), teams.AddRange);
            });
        }
    }
}