using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using CodeFramework.Core.ViewModels;
using BitbucketSharp.Models;

namespace CodeBucket.Core.ViewModels.PullRequests
{
    public class PullRequestViewModel : LoadableViewModel
    {
        private PullRequestModel _model;
        private bool _merged;
		private readonly CollectionViewModel<PullRequestCommentModel> _comments = new CollectionViewModel<PullRequestCommentModel>();

        public string User 
        { 
            get; 
            private set; 
        }

        public string Repo 
        { 
            get; 
            private set; 
        }

        public ulong PullRequestId 
        { 
            get; 
            private set; 
        }

        public bool Merged
        {
            get { return _merged; }
            set { _merged = value; RaisePropertyChanged(() => Merged); }
        }

        public PullRequestModel PullRequest 
        { 
            get { return _model; }
            set
            {
                _model = value;
				_merged = string.Equals(value.State, "fulfilled");
                RaisePropertyChanged(() => PullRequest);
            }
        }

		public CollectionViewModel<PullRequestCommentModel> Comments
        {
            get { return _comments; }
        }

		public ICommand GoToCommitsCommand
		{
			get { return null; }//return new MvxCommand(() => ShowViewModel<PullRequestCommitsViewModel>(new PullRequestCommitsViewModel.NavObject { Username = User, Repository = Repo, PullRequestId = PullRequestId })); }
		}

		public ICommand GoToFilesCommand
		{
			get { return null; }//return new MvxCommand(() => ShowViewModel<PullRequestFilesViewModel>(new PullRequestFilesViewModel.NavObject { Username = User, Repository = Repo, PullRequestId = PullRequestId })); }
		}

        public void Init(NavObject navObject)
        {
            User = navObject.Username;
            Repo = navObject.Repository;
            PullRequestId = navObject.Id;
        }

        protected override Task Load(bool forceCacheInvalidation)
        {
			var t1 = this.RequestModel(() => this.GetApplication().Client.Users[User].Repositories[Repo].PullRequests[PullRequestId].Get(forceCacheInvalidation), response => PullRequest = response);
			Comments.SimpleCollectionLoad(() => this.GetApplication().Client.Users[User].Repositories[Repo].PullRequests[PullRequestId].GetComments()).FireAndForget();
            return t1;
        }

        public async Task AddComment(string text)
        {
			var comment = await Task.Run(() => this.GetApplication().Client.Users[User].Repositories[Repo].PullRequests[PullRequestId].AddComment(text));
			//Comments.Items.Add(comment.Data);
        }

        public async Task Merge()
        {
			await Task.Run(() => this.GetApplication().Client.Users[User].Repositories[Repo].PullRequests[PullRequestId].Merge());
			await this.RequestModel(() => this.GetApplication().Client.Users[User].Repositories[Repo].PullRequests[PullRequestId].Get(true), r => PullRequest = r);
        }

        public ICommand MergeCommand
        {
            get { return new MvxCommand(() => Merge(), CanMerge); }
        }

        private bool CanMerge()
        {
            if (PullRequest == null)
                return false;
			return string.Equals(PullRequest.State, "open");
        }

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
            public ulong Id { get; set; }
        }
    }
}
