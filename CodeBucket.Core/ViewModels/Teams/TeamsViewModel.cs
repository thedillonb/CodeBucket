using System;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive.Linq;
using Splat;
using System.Reactive;
using CodeBucket.Client;

namespace CodeBucket.Core.ViewModels.Teams
{
    public class TeamsViewModel : BaseViewModel, ILoadableViewModel, IListViewModel<TeamItemViewModel>
    {
        public IReadOnlyReactiveList<TeamItemViewModel> Items { get; }

        public IReactiveCommand<Unit> LoadCommand { get; }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _isEmpty;
        public bool IsEmpty => _isEmpty.Value;

        public TeamsViewModel(IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Teams";

            var teams = new ReactiveList<User>();
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
                return applicationService.Client.ForAllItems(x => x.Teams.GetAll(), teams.AddRange);
            });

            _isEmpty = LoadCommand
                .IsExecuting
                .Skip(1)
                .Select(x => !x && teams.Count == 0)
                .ToProperty(this, x => x.IsEmpty);
        }
    }
}