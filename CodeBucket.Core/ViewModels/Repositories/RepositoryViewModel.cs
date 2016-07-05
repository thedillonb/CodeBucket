using System;
using System.Threading.Tasks;
using CodeBucket.Core.ViewModels.Users;
using CodeBucket.Core.ViewModels.Events;
using System.Linq;
using CodeBucket.Core.ViewModels.Commits;
using CodeBucket.Core.Services;
using CodeBucket.Core.Utils;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;
using CodeBucket.Core.ViewModels.PullRequests;
using CodeBucket.Core.ViewModels.Wiki;
using CodeBucket.Core.ViewModels.Source;
using CodeBucket.Core.ViewModels.Issues;
using CodeBucket.Client;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoryViewModel : BaseViewModel, ILoadableViewModel
    {
        private readonly IApplicationService _applicationService;
        private string _readmeFilename;

        private readonly ReactiveList<Client.V1.GitReference> _branches = new ReactiveList<Client.V1.GitReference>();

        private bool _hasReadme;
        public bool HasReadme
        {
            get { return _hasReadme; }
            private set { this.RaiseAndSetIfChanged(ref _hasReadme, value); }
        }

        private Repository _repository;
        public Repository Repository
        {
            get { return _repository; }
            private set { this.RaiseAndSetIfChanged(ref _repository, value); }
        }

        private int? _watchers;
        public int? Watchers
        {
            get { return _watchers; }
            private set { this.RaiseAndSetIfChanged(ref _watchers, value); }
        }

        private int? _forks;
        public int? Forks
        {
            get { return _forks; }
            private set { this.RaiseAndSetIfChanged(ref _forks, value); }
        }

        private readonly ObservableAsPropertyHelper<int> _branchesCount;
        public int BranchesCount => _branchesCount.Value;

        private int? _issues;
        public int? Issues
        {
            get { return _issues; }
            private set { this.RaiseAndSetIfChanged(ref _issues, value); }
        }

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IReactiveCommand<object> GoToOwnerCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToForkParentCommand { get; }

        public IReactiveCommand<object> GoToStargazersCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToEventsCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToIssuesCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToPullRequestsCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToWikiCommand { get; }

        public IReactiveCommand<object> GoToCommitsCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToSourceCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToBranchesCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToWebsiteCommand { get; }

        public IReactiveCommand<Unit> ShowMenuCommand { get; }

        public IReactiveCommand<object> GoToReadmeCommand { get; }

        public IReactiveCommand<Unit> ForkCommand { get; }

        public RepositoryViewModel(string username, string repository,
            IApplicationService applicationService = null, IActionMenuService actionMenuService = null)
        {
            applicationService = _applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            actionMenuService = actionMenuService ?? Locator.Current.GetService<IActionMenuService>();

            _branches.Changed
                .Select(_ => _branches.Count)
                .ToProperty(this, x => x.BranchesCount, out _branchesCount);

            this.WhenAnyValue(x => x.Repository.Name)
                .StartWith(repository)
                .Subscribe(x => Title = x);

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ => {
                Repository = await applicationService.Client.Repositories.Get(username, repository);

                _applicationService.Client.Repositories.GetWatchers(username, repository)
                                   .ToBackground(x => Watchers = x.Size);

                _applicationService.Client.Repositories.GetForks(username, repository)
                    .ToBackground(x => Forks = x.Size);

                _applicationService.Client.Repositories.GetBranches(username, repository)
                    .ToBackground(x => _branches.Reset(x));

                if (!Repository.HasIssues)
                    Issues = 0;
                else
                {
                    _applicationService.Client.Issues.GetAll(username, repository, limit: 0)
                        .ToBackground(x => Issues = x.Count);
                }

                LoadReadme(username, repository).ToBackground();
            });

            ForkCommand = ReactiveCommand.CreateAsyncTask(async _ => {
                var fork = await applicationService.Client.Repositories.Fork(username, repository);
                NavigateTo(new RepositoryViewModel(fork.Owner, fork.Slug));
            });

            var canGoToFork = this.WhenAnyValue(x => x.Repository).Select(x => x.Parent != null);
            GoToForkParentCommand = ReactiveCommand.Create(canGoToFork);
            GoToForkParentCommand
                .Select(_ => RepositoryIdentifier.FromFullName(Repository.Parent.FullName))
                .Select(x => new RepositoryViewModel(x.Owner, x.Name))
                .Subscribe(NavigateTo);

            GoToReadmeCommand = ReactiveCommand.Create(this.WhenAnyValue(x => x.HasReadme));
            GoToReadmeCommand
                .Select(_ => new ReadmeViewModel(username, repository, _readmeFilename))
                .Subscribe(NavigateTo);

            GoToPullRequestsCommand
                .Select(_ => new PullRequestsViewModel(username, repository))
                .Subscribe(NavigateTo);

            GoToWikiCommand = ReactiveCommand.Create(this.WhenAnyValue(x => x.Repository.HasWiki));
            GoToWikiCommand
                .Select(_ => new WikiViewModel(username, repository))
                .Subscribe(NavigateTo);

            GoToSourceCommand
                .Select(_ => new BranchesAndTagsViewModel(username, repository))
                .Subscribe(NavigateTo);

            GoToIssuesCommand
                .Select(_ => new IssuesViewModel(username, repository))
                .Subscribe(NavigateTo);

            GoToOwnerCommand
                .Select(_ => new UserViewModel(username))
                .Subscribe(NavigateTo);

            GoToStargazersCommand
                .Select(_ => new RepositoryWatchersViewModel(username, repository))
                .Subscribe(NavigateTo);

            GoToEventsCommand
                .Select(_ => new RepositoryEventsViewModel(username, repository))
                .Subscribe(NavigateTo);

            GoToBranchesCommand
                .Select(_ => new BranchesViewModel(username, repository))
                .Subscribe(NavigateTo);

            var validWebsite = this.WhenAnyValue(x => x.Repository.Website)
                                   .Select(x => !string.IsNullOrEmpty(x));

            GoToWebsiteCommand = ReactiveCommand.Create(validWebsite);
            GoToWebsiteCommand
                .Select(_ => new WebBrowserViewModel(Repository.Website))
                .Subscribe(NavigateTo);

            GoToCommitsCommand
                .Subscribe(_ =>
                {
                    if (_branches.Count == 1)
                        NavigateTo(new CommitsViewModel(username, repository, _branches.FirstOrDefault()?.Node));
                    else
                        NavigateTo(new BranchesViewModel(username, repository));
                });

            ShowMenuCommand = ReactiveCommand.CreateAsyncTask(sender =>
            {
                var menu = actionMenuService.Create();
                var isPinned = applicationService.Account.PinnnedRepositories.GetPinnedRepository(username, repository) != null;
                var pinned = isPinned ? "Unpin from Slideout Menu" : "Pin to Slideout Menu";
                menu.AddButton(pinned, PinRepository);
                menu.AddButton("Fork Repository", ForkCommand);
                menu.AddButton("Show in Bitbucket", () => {
                    var htmlUrl = ("https://bitbucket.org/" + username + "/" + repository).ToLower();
                    NavigateTo(new WebBrowserViewModel(htmlUrl));
                });
                return menu.Show(sender);
            });
        }

        private void PinRepository()
        {
            var repoInfo = RepositoryIdentifier.FromFullName(Repository.FullName);

            //Is it pinned already or not?
            var pinnedRepo = _applicationService.Account.PinnnedRepositories.GetPinnedRepository(repoInfo.Owner, repoInfo.Name);
            if (pinnedRepo == null)
            {
                var avatar = new Avatar(Repository.Links.Avatar.Href).ToUrl();
                _applicationService.Account.PinnnedRepositories.AddPinnedRepository(repoInfo.Owner, repoInfo.Name, Repository.Name, avatar);
            }
            else
				_applicationService.Account.PinnnedRepositories.RemovePinnedRepository(pinnedRepo.Id);
        }

        private async Task LoadReadme(string username, string repository)
        {
            var mainBranch = await _applicationService.Client.Repositories.GetPrimaryBranch(username, repository);
            var sources = await _applicationService.Client.Repositories.GetSourceDirectory(username, repository, mainBranch.Name);
            var readme = sources.Files.FirstOrDefault(x => x.Path.StartsWith("readme", StringComparison.OrdinalIgnoreCase));
            _readmeFilename = readme?.Path;
            HasReadme = readme != null;
        }
    }
}

