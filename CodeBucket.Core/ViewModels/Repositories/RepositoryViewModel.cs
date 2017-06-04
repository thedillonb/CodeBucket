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

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        public ReactiveCommand<Unit, Unit> GoToOwnerCommand { get; } = ReactiveCommandFactory.Empty();

        public ReactiveCommand<Unit, Unit> GoToForkParentCommand { get; }

        public ReactiveCommand<Unit, Unit> GoToStargazersCommand { get; } = ReactiveCommandFactory.Empty();

        public ReactiveCommand<Unit, Unit> GoToEventsCommand { get; } = ReactiveCommandFactory.Empty();

        public ReactiveCommand<Unit, Unit> GoToIssuesCommand { get; } = ReactiveCommandFactory.Empty();

        public ReactiveCommand<Unit, Unit> GoToPullRequestsCommand { get; } = ReactiveCommandFactory.Empty();

        public ReactiveCommand<Unit, Unit> GoToWikiCommand { get; }

        public ReactiveCommand<Unit, Unit> GoToCommitsCommand { get; } = ReactiveCommandFactory.Empty();

        public ReactiveCommand<Unit, Unit> GoToSourceCommand { get; } = ReactiveCommandFactory.Empty();

        public ReactiveCommand<Unit, Unit> GoToBranchesCommand { get; } = ReactiveCommandFactory.Empty();

        public ReactiveCommand<Unit, Unit> GoToWebsiteCommand { get; }

        public ReactiveCommand<Unit, Unit> ShowMenuCommand { get; }

        public ReactiveCommand<Unit, Unit> GoToReadmeCommand { get; }

        public ReactiveCommand<Unit, Unit> ForkCommand { get; }

        public RepositoryViewModel(
            string username, string repositoryName, Repository repository = null,
            IApplicationService applicationService = null, IActionMenuService actionMenuService = null)
        {
            applicationService = _applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            actionMenuService = actionMenuService ?? Locator.Current.GetService<IActionMenuService>();

            Repository = repository;

            _branches.Changed
                .Select(_ => _branches.Count)
                .ToProperty(this, x => x.BranchesCount, out _branchesCount);

            this.WhenAnyValue(x => x.Repository.Name)
                .StartWith(repositoryName)
                .Subscribe(x => Title = x);

            LoadCommand = ReactiveCommand.CreateFromTask(async _ => {
                Repository = await applicationService.Client.Repositories.Get(username, repositoryName);

                _applicationService.Client.Repositories.GetWatchers(username, repositoryName)
                                   .ToBackground(x => Watchers = x.Size);

                _applicationService.Client.Repositories.GetForks(username, repositoryName)
                    .ToBackground(x => Forks = x.Size);

                _applicationService.Client.Repositories.GetBranches(username, repositoryName)
                    .ToBackground(x => _branches.Reset(x));

                if (!Repository.HasIssues)
                    Issues = 0;
                else
                {
                    _applicationService.Client.Issues.GetAll(username, repositoryName, limit: 0)
                        .ToBackground(x => Issues = x.Count);
                }

                LoadReadme(username, repositoryName).ToBackground();
            });

            ForkCommand = ReactiveCommand.CreateFromTask(async _ => {
                var fork = await applicationService.Client.Repositories.Fork(username, repositoryName);
                NavigateTo(new RepositoryViewModel(fork.Owner, fork.Slug));
            });

            var canGoToFork = this.WhenAnyValue(x => x.Repository)
                                  .Select(x => x?.Parent != null);
            
            GoToForkParentCommand = ReactiveCommand.Create(() => {
                var id = RepositoryIdentifier.FromFullName(Repository.Parent.FullName);
                NavigateTo(new RepositoryViewModel(id.Owner, id.Name));
            }, canGoToFork);

            GoToReadmeCommand = ReactiveCommand.Create(
                () => NavigateTo(new ReadmeViewModel(username, repositoryName, _readmeFilename)),
                this.WhenAnyValue(x => x.HasReadme));

            GoToPullRequestsCommand
                .Select(_ => new PullRequestsViewModel(username, repositoryName))
                .Subscribe(NavigateTo);

            GoToWikiCommand = ReactiveCommand.Create(
                () => NavigateTo(new WikiViewModel(username, repositoryName)),
                this.WhenAnyValue(x => x.Repository.HasWiki));

            GoToSourceCommand
                .Select(_ => new BranchesAndTagsViewModel(username, repositoryName))
                .Subscribe(NavigateTo);

            GoToIssuesCommand
                .Select(_ => new IssuesViewModel(username, repositoryName))
                .Subscribe(NavigateTo);

            GoToOwnerCommand
                .Select(_ => new UserViewModel(username))
                .Subscribe(NavigateTo);

            GoToStargazersCommand
                .Select(_ => new RepositoryWatchersViewModel(username, repositoryName))
                .Subscribe(NavigateTo);

            GoToEventsCommand
                .Select(_ => new RepositoryEventsViewModel(username, repositoryName))
                .Subscribe(NavigateTo);

            GoToBranchesCommand
                .Select(_ => BranchesViewModel.ForSource(username, repositoryName))
                .Subscribe(NavigateTo);

            var validWebsite = this.WhenAnyValue(x => x.Repository.Website)
                                   .Select(x => !string.IsNullOrEmpty(x));

            GoToWebsiteCommand = ReactiveCommand.Create(
                () => NavigateTo(new WebBrowserViewModel(Repository.Website)),
                validWebsite);

            GoToCommitsCommand
                .Subscribe(_ =>
                {
                    if (_branches.Count == 1)
                        NavigateTo(new CommitsViewModel(username, repositoryName, _branches.FirstOrDefault()?.Node));
                    else
                        NavigateTo(BranchesViewModel.ForCommits(username, repositoryName));
                });

            ShowMenuCommand = ReactiveCommand.CreateFromTask(sender =>
            {
                var menu = actionMenuService.Create();
                var isPinned = applicationService
                    .Account.PinnedRepositories
                    .Any(x => string.Equals(x.Owner, username, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Slug, repositoryName, StringComparison.OrdinalIgnoreCase));
                var pinned = isPinned ? "Unpin from Slideout Menu" : "Pin to Slideout Menu";
                menu.AddButton(pinned, _ => PinRepository());
                menu.AddButton("Fork Repository", _ => ForkCommand.ExecuteNow());
                menu.AddButton("Show in Bitbucket", _ => 
                {
                    var htmlUrl = ("https://bitbucket.org/" + username + "/" + repositoryName).ToLower();
                    NavigateTo(new WebBrowserViewModel(htmlUrl));
                });
                return menu.Show(sender);
            });
        }

        private void PinRepository()
        {
            var repoInfo = RepositoryIdentifier.FromFullName(Repository.FullName);

            //Is it pinned already or not?
            var pinnedRepo = _applicationService.Account.PinnedRepositories.Find(
                x => string.Equals(x.Owner, repoInfo.Owner, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(x.Slug, repoInfo.Name, StringComparison.OrdinalIgnoreCase));
            if (pinnedRepo == null)
            {
                var avatar = new Avatar(Repository.Links.Avatar.Href).ToUrl();
                _applicationService.Account.PinnedRepositories.Add(new Data.PinnedRepository
                {
                    Owner = repoInfo.Owner,
                    Slug = repoInfo.Name,
                    ImageUri = avatar,
                    Name = repoInfo.Name
                });
            }
            else
                _applicationService.Account.PinnedRepositories.RemoveAll(x => x.Id == pinnedRepo.Id);
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

