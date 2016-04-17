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
using System.Reactive.Linq;
using System.Reactive;
using System.Linq;
using BitbucketSharp;

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

        public ReactiveUI.IReactiveCommand<Unit> ToggleApproveButton { get; }

        public ReactiveUI.IReactiveCommand<object> GoToUserCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public ReactiveUI.IReactiveCommand<object> GoToRepositoryCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public ReactiveUI.IReactiveCommand<object> GoToFileCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public ReactiveUI.IReactiveCommand<object> GoToAddedFiles { get; } = ReactiveUI.ReactiveCommand.Create();

        public ReactiveUI.IReactiveCommand<object> GoToRemovedFiles { get; } = ReactiveUI.ReactiveCommand.Create();

        public ReactiveUI.IReactiveCommand<object> GoToModifiedFiles { get; } = ReactiveUI.ReactiveCommand.Create();

        public ReactiveUI.IReactiveCommand<object> GoToAllFiles { get; } = ReactiveUI.ReactiveCommand.Create();

        public ReactiveUI.IReactiveCommand<object> ShowMenuCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public ReactiveUI.ReactiveList<CommitFileViewModel> CommitFiles { get; } = new ReactiveUI.ReactiveList<CommitFileViewModel>();

        private ChangesetModel _changeset;
        public ChangesetModel Changeset
        {
            get { return _changeset; }
            private set { this.RaiseAndSetIfChanged(ref _changeset, value); }
        }

        private Commit _commit;
        public Commit Commit
		{
			get { return _commit; }
            private set { this.RaiseAndSetIfChanged(ref _commit, value); }
		}

        private IReadOnlyList<CommitComment> _comments;
        public IReadOnlyList<CommitComment> Comments
        {
            get { return _comments; }
            private set { this.RaiseAndSetIfChanged(ref _comments, value); }
        }

        private int _diffAdditions;
        public int DiffAdditions
        {
            get { return _diffAdditions; }
            private set { this.RaiseAndSetIfChanged(ref _diffAdditions, value); }
        }

        private int _diffDeletions;
        public int DiffDeletions
        {
            get { return _diffDeletions; }
            private set { this.RaiseAndSetIfChanged(ref _diffDeletions, value); }
        }

        private int _diffModifications;
        public int DiffModifications
        {
            get { return _diffModifications; }
            private set { this.RaiseAndSetIfChanged(ref _diffModifications, value); }
        }

        private bool _approved;
        public bool Approved
        {
            get { return _approved; }
            private set { this.RaiseAndSetIfChanged(ref _approved, value); }
        }

        public CommitViewModel(IApplicationService applicationService)
        {
            _applicationService = applicationService;

            GoToUserCommand
                .OfType<string>()
                .Select(x => new UserViewModel.NavObject { Username = x })
                .Subscribe(x => ShowViewModel<UserViewModel>(x));

            GoToRepositoryCommand
                .Select(_ => new RepositoryViewModel.NavObject { Username = User, RepositorySlug = Repository })
                .Subscribe(x => ShowViewModel<RepositoryViewModel>(x));

            GoToFileCommand
                .OfType<ChangesetDiffModel>()
                .Subscribe(x => {
                    Mvx.Resolve<IViewModelTxService>().Add(x);
                    ShowViewModel<ChangesetDiffViewModel>(new ChangesetDiffViewModel.NavObject { Username = User, Repository = Repository, Branch = Node, Filename = x.File });
                });

            ToggleApproveButton = ReactiveUI.ReactiveCommand.CreateAsyncTask(async _ => 
            {
                if (Approved)
                    await _applicationService.Client.Repositories.UnapproveCommit(User, Repository, Node);
                else
                    await _applicationService.Client.Repositories.ApproveCommit(User, Repository, Node);

                Commit = await _applicationService.Client.Repositories.GetCommit(User, Repository, Node);
            });

            ToggleApproveButton.ThrownExceptions
                .Subscribe(x => DisplayAlert("Unable to approve commit: " + x.Message).ToBackground());

            this.Bind(x => x.Changeset, true).Subscribe(x => {
                DiffAdditions = x?.Files.Count(y => y.Type == ChangesetModel.FileType.Added) ?? 0;
                DiffDeletions = x?.Files.Count(y => y.Type == ChangesetModel.FileType.Removed) ?? 0;
                DiffModifications = x?.Files.Count(y => y.Type == ChangesetModel.FileType.Modified) ?? 0;

                var files = (x?.Files ?? Enumerable.Empty<ChangesetModel.FileModel>()).Select(y =>
                {
                    var vm = new CommitFileViewModel(y.File, y.Type);
                    return vm;
                });

                ReactiveUI.ReactiveListExtensions.Reset(CommitFiles, files);
            });

            this.Bind(x => x.Commit, true).Subscribe(x => {
                var username = applicationService.Account.Username;
                Approved = x?.Participants
                    .FirstOrDefault(y => string.Equals(y.User.Username, username, StringComparison.OrdinalIgnoreCase))
                    ?.Approved ?? false;
            });

            LoadCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(_ => {
                var commit = applicationService.Client.Repositories.GetCommit(User, Repository, Node)
                    .OnSuccess(x => Commit = x);
                var changeset = applicationService.Client.Repositories.GetChangeset(User, Repository, Node)
                    .OnSuccess(x => Changeset = x);
     
                applicationService.Client.AllItems(x => x.Repositories.GetCommitComments(User, Repository, Node))
                    .ToBackground(x => Comments = x.ToList());

                return Task.WhenAll(commit, changeset);
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

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
            public string Node { get; set; }
            public bool ShowRepository { get; set; }
        }
    }
}

