using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using CodeBucket.Core.ViewModels.Repositories;
using System.Threading.Tasks;
using CodeBucket.Core.ViewModels.Source;
using BitbucketSharp.Models;
using System.Collections.Generic;
using System;
using CodeBucket.Core.ViewModels.User;
using CodeBucket.Core.Services;
using BitbucketSharp.Models.V2;
using MvvmCross.Platform;

namespace CodeBucket.Core.ViewModels.Commits
{
    public class CommitViewModel : BaseViewModel, ILoadableViewModel
    {
        private readonly IApplicationService _applicationService;

		public string Node { get; private set; }

		public string User { get; private set; }

		public string Repository { get; private set; }

        public bool ShowRepository { get; private set; }

        public ReactiveUI.IReactiveCommand LoadCommand { get; }

		private List<ChangesetDiffModel> _commitModel;
		public List<ChangesetDiffModel> Commits
        {
            get { return _commitModel; }
            private set { this.RaiseAndSetIfChanged(ref _commitModel, value); }
        }

        private Commit _commit;
        public Commit Commit
		{
			get { return _commit; }
            private set { this.RaiseAndSetIfChanged(ref _commit, value); }
		}

		public ICommand GoToUserCommand
		{
			get { return new MvxCommand<string>(x => ShowViewModel<ProfileViewModel>(new ProfileViewModel.NavObject { Username = x })); }
		}

        public ICommand GoToRepositoryCommand
        {
            get { return new MvxCommand(() => ShowViewModel<RepositoryViewModel>(new RepositoryViewModel.NavObject { Username = User, RepositorySlug = Repository })); }
        }

		public ICommand GoToFileCommand
		{
			get
			{ 
				return new MvxCommand<ChangesetDiffModel>(x =>
				{
//						if (x. == null)
//						{
//							ShowViewModel<SourceViewModel>(new SourceViewModel.NavObject { GitUrl = x.ContentsUrl, HtmlUrl = x.BlobUrl, Name = x.Filename, Path = x.Filename, ForceBinary = true });
//						}
//						else
//						{
						Mvx.Resolve<IViewModelTxService>().Add(x);
						ShowViewModel<ChangesetDiffViewModel>(new ChangesetDiffViewModel.NavObject { Username = User, Repository = Repository, Branch = Node, Filename = x.File });
//						}

				});
			}
		}

        public CollectionViewModel<CommitComment> Comments { get; } = new CollectionViewModel<CommitComment>();

        public CommitViewModel(IApplicationService applicationService)
        {
            _applicationService = applicationService;


            LoadCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(async _ =>
            {
                Commit = await applicationService.Client.Repositories.GetCommit(User, Repository, Node);
            });
        }

        public void Init(NavObject navObject)
        {
            User = navObject.Username;
            Repository = navObject.Repository;
            Node = navObject.Node;
            ShowRepository = navObject.ShowRepository;
        }

//		protected override async Task Load(bool forceCacheInvalidation)
//        {
//			var t1 = this.RequestModel(() => this.GetApplication().Client.Users[User].Repositories[Repository].Changesets[Node].GetDiffs(forceCacheInvalidation), response => Commits = response);
//            var t2 = this.RequestModel(() => this.GetApplication().Client.Users[User].Repositories[Repository].Changesets[Node].GetCommit(), response => Commit = response);
//			await Task.WhenAll(t1, t2);
//            GetAllComments().FireAndForget();
//        }

        private async Task GetAllComments()
        {
//            var comments = new List<CommitComment>();
//            var ret = await Task.Run(() => this.GetApplication().Client.Users[User].Repositories[Repository].Changesets[Node].GetComments());
//            comments.AddRange(ret.Values);
//
//            while (ret.Next != null)
//            {
//                ret = await Task.Run(() => this.GetApplication().Client.Request2<Collection<CommitComment>>(ret.Next));
//                comments.AddRange(ret.Values);
//            }
//
//            Comments.Items.Reset(comments.OrderBy(x => x.CreatedOn));
        }

        public async Task AddComment(string text)
        {
//			try
//			{
//				await Task.Run(() => this.GetApplication().Client.Users[User].Repositories[Repository].Changesets[Node].Comments.Create(text));
//                await GetAllComments();
//			}
//			catch (Exception e)
//			{
//                DisplayAlert("Unable to add comment: " + e.Message).FireAndForget();
//			}
        }

		public async Task Approve()
		{
			try
			{
                await _applicationService.Client.Repositories.ApproveCommit(User, Repository, Node);

			}
			catch (Exception e)
			{
                DisplayAlert("Unable to approve commit: " + e.Message).FireAndForget();
			}
		}

		public async Task Unapprove()
		{
			try
			{
                await _applicationService.Client.Repositories.UnapproveCommit(User, Repository, Node);
			}
			catch (Exception e)
			{
                DisplayAlert("Unable to unapprove commit: " + e.Message).FireAndForget();
			}
		}

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
            public string Node { get; set; }
            public bool ShowRepository { get; set; }
        }
    }
}

