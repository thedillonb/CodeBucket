using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using CodeBucket.Core.ViewModels.User;
using CodeBucket.Core.ViewModels.Events;
using BitbucketSharp.Models;
using System.Linq;
using CodeBucket.Core.ViewModels.Commits;
using CodeBucket.Core.Services;
using CodeBucket.Core.Utils;
using System.Reactive;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoryViewModel : BaseViewModel, ILoadableViewModel
    {
        private readonly IApplicationService _applicationService;
        private string _readmeFilename;

		public string Username { get; private set; }

		public string HtmlUrl
		{
			get { return ("https://bitbucket.org/" + Username + "/" + RepositorySlug).ToLower(); }
		}

		public string RepositorySlug { get; private set; }

        private bool _hasReadme;
        public bool HasReadme
        {
            get { return _hasReadme; }
            private set { this.RaiseAndSetIfChanged(ref _hasReadme, value); }
        }

        private BitbucketSharp.Models.V2.Repository _repository;
        public BitbucketSharp.Models.V2.Repository Repository
        {
            get { return _repository; }
            private set { this.RaiseAndSetIfChanged(ref _repository, value); }
        }

        private List<BranchModel> _branches;
        public List<BranchModel> Branches
        {
            get { return _branches; }
            private set { this.RaiseAndSetIfChanged(ref _branches, value); }
        }

        private int _issues;
        public int Issues
        {
            get { return _issues; }
            private set { this.RaiseAndSetIfChanged(ref _issues, value); }
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
			RepositorySlug = navObject.RepositorySlug;
        }

        public ReactiveUI.IReactiveCommand LoadCommand { get; }

		public ICommand GoToOwnerCommand
		{
			get { return new MvxCommand(() => ShowViewModel<ProfileViewModel>(new ProfileViewModel.NavObject { Username = Username })); }
		}

        public ReactiveUI.IReactiveCommand<object> GoToForkParentCommand { get; }

		public ICommand GoToStargazersCommand
		{
			get { return new MvxCommand(() => ShowViewModel<WatchersViewModel>(new WatchersViewModel.NavObject { User = Username, Repository = RepositorySlug })); }
		}

		public ICommand GoToEventsCommand
		{
			get { return new MvxCommand(() => ShowViewModel<RepositoryEventsViewModel>(new RepositoryEventsViewModel.NavObject { Username = Username, Repository = RepositorySlug })); }
		}

		public ICommand GoToIssuesCommand
		{
			get { return new MvxCommand(() => ShowViewModel<Issues.IssuesViewModel>(new Issues.IssuesViewModel.NavObject { Username = Username, Repository = RepositorySlug })); }
		}

		public ICommand GoToPullRequestsCommand
		{
			get { return new MvxCommand(() => ShowViewModel<PullRequests.PullRequestsViewModel>(new PullRequests.PullRequestsViewModel.NavObject { Username = Username, Repository = RepositorySlug })); }
		}

		public ICommand GoToWikiCommand
		{
			get { return new MvxCommand(() => ShowViewModel<Wiki.WikiViewModel>(new Wiki.WikiViewModel.NavObject { Username = Username, Repository = RepositorySlug })); }
		}

        public ICommand GoToCommitsCommand
        {
            get { return new MvxCommand(ShowCommits);}
        }

		public ICommand GoToSourceCommand
		{
			get { return new MvxCommand(() => ShowViewModel<Source.BranchesAndTagsViewModel>(new Source.BranchesAndTagsViewModel.NavObject { Username = Username, Repository = RepositorySlug })); }
		}


        public ICommand GoToReadmeCommand
        {
            get 
            { 
                return new MvxCommand(() => ShowViewModel<ReadmeViewModel>(
                    new ReadmeViewModel.NavObject { 
                        Username = Username, 
                        Repository = RepositorySlug, 
                        Filename = _readmeFilename 
                    }), () => !string.IsNullOrEmpty(_readmeFilename)); 
            }
        }


        private void ShowCommits()
        {
            if (Branches != null && Branches.Count == 1)
                ShowViewModel<CommitsViewModel>(new CommitsViewModel.NavObject {Username = Username, Repository = RepositorySlug});
            else
				ShowViewModel<Source.BranchesViewModel>(new Source.BranchesViewModel.NavObject {Username = Username, Repository = RepositorySlug});
        }
		
        public ICommand PinCommand
        {
            get { return new MvxCommand(PinRepository, () => Repository != null); }
        }

        public ReactiveUI.IReactiveCommand<Unit> ForkCommand { get; }

        public RepositoryViewModel(IApplicationService applicationService)
        {
            _applicationService = applicationService;

            LoadCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(async _ =>
            {
                Repository = await applicationService.Client.Repositories.GetRepository(Username, RepositorySlug);

                LoadReadme().ToBackground();
                LoadBranches().ToBackground();
            });

            ForkCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var fork = await applicationService.Client.Repositories.Fork(Username, RepositorySlug);
                ShowViewModel<RepositoryViewModel>(new RepositoryViewModel.NavObject { Username = fork.Owner, RepositorySlug = fork.Slug });
            });

            var canGoToFork = this.Bind(x => x.Repository, true).Select(x => x.Parent != null);
            GoToForkParentCommand = ReactiveUI.ReactiveCommand.Create(canGoToFork);
            GoToForkParentCommand
                .Select(_ => new RepositoryIdentifier(Repository.Parent.Fullname))
                .Select(x => new RepositoryViewModel.NavObject { Username = x.Owner, RepositorySlug = x.Name })
                .Subscribe(x => ShowViewModel<RepositoryViewModel>(x));
        }

        private void PinRepository()
        {
            var repoInfo = new RepositoryIdentifier(Repository.FullName);

            //Is it pinned already or not?
            var pinnedRepo = _applicationService.Account.PinnnedRepositories.GetPinnedRepository(repoInfo.Owner, repoInfo.Name);
            if (pinnedRepo == null)
            {
                var avatar = new Avatar(Repository.Links.Avatar.Href).ToUrl();
                this.GetApplication().Account.PinnnedRepositories.AddPinnedRepository(repoInfo.Owner, repoInfo.Name, Repository.Name, avatar);
            }
            else
				this.GetApplication().Account.PinnnedRepositories.RemovePinnedRepository(pinnedRepo.Id);
        }

//
//        protected override Task Load(bool forceCacheInvalidation)
//        {
//            Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[RepositorySlug].Issues.GetIssues(0, 0))
//                .ContinueWith(x => { Issues = x.Result.Count; }, TaskContinuationOptions.OnlyOnRanToCompletion);
//
//            return t1;
//        }
//
        private async Task LoadBranches()
        {
            var branches = await _applicationService.Client.Repositories.GetBranches(Username, RepositorySlug);
            Branches = branches.Values.ToList();
        }

        private async Task LoadIssues()
        {
            
        }

        private async Task LoadReadme()
        {
            var mainBranch = await _applicationService.Client.Repositories.GetMainBranch(Username, RepositorySlug);
            var sources = await _applicationService.Client.Repositories.GetSource(Username, RepositorySlug, mainBranch.Name);
            var readme = sources.Files.FirstOrDefault(x => x.Path.StartsWith("readme", StringComparison.OrdinalIgnoreCase));
            _readmeFilename = readme.Path;
            HasReadme = readme != null;
        }

        public bool IsPinned
        {
			get { return this.GetApplication().Account.PinnnedRepositories.GetPinnedRepository(Username, RepositorySlug) != null; }
        }

        public class NavObject
        {
            public string Username { get; set; }
            public string RepositorySlug { get; set; }
        }
    }
}

