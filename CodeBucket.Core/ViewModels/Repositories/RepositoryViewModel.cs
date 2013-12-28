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

        public string Username 
        { 
            get; 
            private set; 
        }

        public string RepositoryName 
        { 
            get; 
            private set; 
        }

		public string ImageUrl
		{
			get;
			set;
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
            RepositoryName = navObject.Repository;
        }

		public ICommand GoToOwnerCommand
		{
			get { return new MvxCommand(() => ShowViewModel<ProfileViewModel>(new ProfileViewModel.NavObject { Username = Username })); }
		}

		public ICommand GoToForkParentCommand
		{
			get { return new MvxCommand<RepositoryDetailedModel>(x => ShowViewModel<RepositoryViewModel>(new RepositoryViewModel.NavObject { Username = x.Owner, Repository = x.Name })); }
		}

		public ICommand GoToStargazersCommand
		{
			get { return new MvxCommand(() => ShowViewModel<WatchersViewModel>(new WatchersViewModel.NavObject { User = Username, Repository = RepositoryName })); }
		}

		public ICommand GoToEventsCommand
		{
			get { return new MvxCommand(() => ShowViewModel<RepositoryEventsViewModel>(new RepositoryEventsViewModel.NavObject { Username = Username, Repository = RepositoryName })); }
		}

		public ICommand GoToIssuesCommand
		{
			get { return new MvxCommand(() => ShowViewModel<Issues.IssuesViewModel>(new Issues.IssuesViewModel.NavObject { Username = Username, Repository = RepositoryName })); }
		}

		public ICommand GoToPullRequestsCommand
		{
			get { return new MvxCommand(() => ShowViewModel<PullRequests.PullRequestsViewModel>(new PullRequests.PullRequestsViewModel.NavObject { Username = Username, Repository = RepositoryName })); }
		}

        public ICommand GoToCommitsCommand
        {
            get { return new MvxCommand(ShowCommits);}
        }

		public ICommand GoToSourceCommand
		{
			get { return new MvxCommand(() => ShowViewModel<Source.BranchesAndTagsViewModel>(new Source.BranchesAndTagsViewModel.NavObject { Username = Username, Repository = RepositoryName })); }
		}

        private void ShowCommits()
        {
            if (Branches != null && Branches.Count == 1)
                ShowViewModel<ChangesetsViewModel>(new ChangesetsViewModel.NavObject {Username = Username, Repository = RepositoryName});
            else
				ShowViewModel<Source.ChangesetBranchesViewModel>(new Source.ChangesetBranchesViewModel.NavObject {Username = Username, Repository = RepositoryName});
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
			var pinnedRepo = this.GetApplication().Account.PinnnedRepositories.GetPinnedRepository(repoOwner, repoName);
            if (pinnedRepo == null)
				this.GetApplication().Account.PinnnedRepositories.AddPinnedRepository(repoOwner, repoName, repoName, ImageUrl);
            else
				this.GetApplication().Account.PinnnedRepositories.RemovePinnedRepository(pinnedRepo.Id);
        }


        protected override Task Load(bool forceCacheInvalidation)
        {
			var t1 = this.RequestModel(() => this.GetApplication().Client.Users[Username].Repositories[RepositoryName].GetInfo(forceCacheInvalidation), response => Repository = response);

			this.RequestModel(() => this.GetApplication().Client.Users[Username].Repositories[RepositoryName].Branches.GetBranches(forceCacheInvalidation), 
				response => Branches = response.Values.ToList()).FireAndForget();
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
			get { return this.GetApplication().Account.PinnnedRepositories.GetPinnedRepository(Username, RepositoryName) != null; }
        }

		public ICommand ForkCommand
		{
			get { return new MvxCommand<string>(x => Fork(x), x => !string.IsNullOrEmpty(x)); }
		}
		
		public async Task Fork(string name)
		{
			try
			{
				var fork = await Task.Run(() => this.GetApplication().Client.Users[Repository.Owner].Repositories[Repository.Name].ForkRepository(name));
				ShowViewModel<RepositoryViewModel>(new RepositoryViewModel.NavObject { Username = fork.Owner, Repository = fork.Name });
			}
			catch (Exception e)
			{
				ReportError(e);
			}
		}

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
        }
    }
}

