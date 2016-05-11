using System;
using BitbucketSharp.Models.V2;
using CodeBucket.Core.Services;
using BitbucketSharp;
using System.Reactive.Linq;
using CodeBucket.Core.Utils;
using Humanizer;
using System.Linq;
using Splat;
using ReactiveUI;
using System.Reactive;

namespace CodeBucket.Core.ViewModels.PullRequests
{
    public class PullRequestsViewModel : BaseViewModel, ILoadableViewModel
    {
        public IReadOnlyReactiveList<PullRequestItemViewModel> PullRequests { get; }

        public IReactiveCommand<Unit> LoadCommand { get; }

		private PullRequestState _selectedFilter;
		public PullRequestState SelectedFilter
		{
			get { return _selectedFilter; }
            set { this.RaiseAndSetIfChanged(ref _selectedFilter, value); }
		}

        public PullRequestsViewModel(string username, string repository,
                                     IApplicationService applicationService = null)
		{
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Pull Requests";

            var pullRequests = new ReactiveList<PullRequest>();
            PullRequests = pullRequests.CreateDerivedCollection(pullRequest =>
            {
                var avatar = new Avatar(pullRequest.Author?.Links?.Avatar?.Href);
                var vm = new PullRequestItemViewModel(pullRequest.Title, avatar, pullRequest.CreatedOn.Humanize());
                vm.GoToCommand
                  .Select(_ => new PullRequestViewModel(username, repository, pullRequest))
                  .Subscribe(NavigateTo);
                return vm;
            });

            LoadCommand = ReactiveCommand.CreateAsyncTask(_ =>
            {
                PullRequests.Reset();
                return applicationService.Client.ForAllItems(x => 
                    x.Repositories.PullRequests.GetPullRequests(username, repository, SelectedFilter), 
                    pullRequests.AddRange);
            });

            this.WhenAnyValue(x => x.SelectedFilter)
                .Skip(1)
                .InvokeCommand(LoadCommand);
        }
    }
}
