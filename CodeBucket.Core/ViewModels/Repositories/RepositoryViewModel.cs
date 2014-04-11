using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using CodeFramework.Core.ViewModels;
using CodeBucket.Core.ViewModels.User;
using CodeBucket.Core.ViewModels.Events;
using BitbucketSharp.Models;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoryViewModel : LoadableViewModel
    {
		private RepositoryDetailedModel _repository;
        private List<BranchModel> _branches;
        private bool _hasReadme;
        private string _primaryBranch;
        private string _readmeFilename;

		public string Username { get; private set; }

		public string HtmlUrl
		{
			get { return ("https://bitbucket.org/" + Username + "/" + RepositorySlug).ToLower(); }
		}

		public string RepositorySlug { get; private set; }

		public string ImageUrl
		{
			get;
			set;
		}

        public bool HasReadme
        {
            get { return _hasReadme; }
            private set
            {
                _hasReadme = value;
                RaisePropertyChanged(() => HasReadme);
            }
        }

		public RepositoryDetailedModel Repository
        {
            get { return _repository; }
            private set
            {
                _repository = value;
                RaisePropertyChanged(() => Repository);
            }
        }

        public List<BranchModel> Branches
        {
            get { return _branches; }
            private set
            {
                _branches = value;
                RaisePropertyChanged(() => Branches);
            }
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
			RepositorySlug = navObject.RepositorySlug;
        }

		public ICommand GoToOwnerCommand
		{
			get { return new MvxCommand(() => ShowViewModel<ProfileViewModel>(new ProfileViewModel.NavObject { Username = Username })); }
		}

		public ICommand GoToForkParentCommand
		{
			get { return new MvxCommand<RepositoryDetailedModel>(x => ShowViewModel<RepositoryViewModel>(new RepositoryViewModel.NavObject { Username = x.Owner, RepositorySlug = x.Slug })); }
		}

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
                        Branch = _primaryBranch, 
                        Filename = _readmeFilename 
                    }), () => !string.IsNullOrEmpty(_primaryBranch) && !string.IsNullOrEmpty(_readmeFilename)); 
            }
        }


        private void ShowCommits()
        {
            if (Branches != null && Branches.Count == 1)
                ShowViewModel<ChangesetsViewModel>(new ChangesetsViewModel.NavObject {Username = Username, Repository = RepositorySlug});
            else
				ShowViewModel<Source.ChangesetBranchesViewModel>(new Source.ChangesetBranchesViewModel.NavObject {Username = Username, Repository = RepositorySlug});
        }
		
        public ICommand PinCommand
        {
            get { return new MvxCommand(PinRepository, () => Repository != null); }
        }

        private void PinRepository()
        {
            var repoOwner = Repository.Owner;
			var repoName = Repository.Name;

            //Is it pinned already or not?
			var pinnedRepo = this.GetApplication().Account.PinnnedRepositories.GetPinnedRepository(repoOwner, Repository.Slug);
            if (pinnedRepo == null)
                this.GetApplication().Account.PinnnedRepositories.AddPinnedRepository(repoOwner, Repository.Slug, repoName, ImageUrl);
            else
				this.GetApplication().Account.PinnnedRepositories.RemovePinnedRepository(pinnedRepo.Id);
        }


        protected override Task Load(bool forceCacheInvalidation)
        {
			var t1 = this.RequestModel(() => this.GetApplication().Client.Users[Username].Repositories[RepositorySlug].GetInfo(forceCacheInvalidation), response => Repository = response);

			this.RequestModel(() => this.GetApplication().Client.Users[Username].Repositories[RepositorySlug].Branches.GetBranches(forceCacheInvalidation), 
				response => Branches = response.Values.ToList()).FireAndForget();

            var uiThread = Cirrious.CrossCore.Mvx.Resolve<CodeFramework.Core.Services.IUIThreadService>();
            Task.Run(() =>
            {
                var primaryBranch = this.GetApplication().Client.Users[Username].Repositories[RepositorySlug].GetPrimaryBranch(true);
                _primaryBranch = primaryBranch.Name;
                var data = this.GetApplication().Client.Users[Username].Repositories[RepositorySlug].Branches[primaryBranch.Name].Source[""].GetInfo(forceCacheInvalidation);
                var any = data.Files.FirstOrDefault(x => x.Path.Substring(x.Path.LastIndexOf("/", StringComparison.Ordinal) + 1).ToLower().StartsWith("readme"));
                if (any != null)
                {
                    _readmeFilename = any.Path;
                    uiThread.MarshalOnUIThread(() => HasReadme = true);
                }
            }).FireAndForget();
//
//			this.RequestModel(this.GetApplication().Client.Users[Username].Repositories[RepositoryName].IsWatching(), 
//				forceCacheInvalidation, response => IsWatched = response.Data).FireAndForget();
//         
//			this.RequestModel(this.GetApplication().Client.Users[Username].Repositories[RepositoryName].IsStarred(), 
//				forceCacheInvalidation, response => IsStarred = response.Data).FireAndForget();

            return t1;
        }

        public bool IsPinned
        {
			get { return this.GetApplication().Account.PinnnedRepositories.GetPinnedRepository(Username, RepositorySlug) != null; }
        }

		public ICommand ForkCommand
		{
			get { return new MvxCommand(() => PromptFork()); }
		}

        private async Task PromptFork()
        {
            try
            {
                var alertSerivce = GetService<CodeFramework.Core.Services.IAlertDialogService>();
                var name = await alertSerivce.PromptTextBox("Fork", "What would you like to name your fork?", Repository.Name, "Fork!");
                await Fork(name);
            }
            catch (TaskCanceledException e)
            {
                // Nothing to see here...
            }
        }
		
		public async Task Fork(string name)
		{
			try
			{
                IsLoading = true;
				var fork = await Task.Run(() => this.GetApplication().Client.Users[Repository.Owner].Repositories[Repository.Name].ForkRepository(name));
				ShowViewModel<RepositoryViewModel>(new RepositoryViewModel.NavObject { Username = fork.Owner, RepositorySlug = fork.Slug });
			}
			catch (Exception e)
			{
                DisplayAlert("Unable to successfully fork the repository: " + e.Message);
			}
            finally
            {
                IsLoading = false;
            }
		}

        public class NavObject
        {
            public string Username { get; set; }
            public string RepositorySlug { get; set; }
        }
    }
}

