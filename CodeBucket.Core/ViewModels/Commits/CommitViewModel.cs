using CodeBucket.Core.ViewModels.Repositories;
using System.Threading.Tasks;
using CodeBucket.Core.ViewModels.Source;
using BitbucketSharp.Models;
using System.Collections.Generic;
using System;
using CodeBucket.Core.ViewModels.Users;
using CodeBucket.Core.Services;
using BitbucketSharp.Models.V2;
using System.Reactive.Linq;
using System.Reactive;
using System.Linq;
using BitbucketSharp;
using ReactiveUI;
using Splat;

namespace CodeBucket.Core.ViewModels.Commits
{
    public class CommitViewModel : BaseViewModel, ILoadableViewModel
    {
        private readonly IApplicationService _applicationService;

		public string Node { get; }

		public string User { get; }

		public string Repository { get; }

        public bool ShowRepository { get; }

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IReactiveCommand<Unit> ToggleApproveButton { get; }

        public IReactiveCommand<object> GoToUserCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToRepositoryCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToAddedFiles { get; }

        public IReactiveCommand<object> GoToRemovedFiles { get; }

        public IReactiveCommand<object> GoToModifiedFiles { get; }

        public IReactiveCommand<object> GoToAllFiles { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> ShowMenuCommand { get; }

        public IReactiveCommand<object> AddCommentCommand { get; } = ReactiveCommand.Create();

        public IReadOnlyReactiveList<CommitFileItemViewModel> CommitFiles { get; }

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

        private readonly ObservableAsPropertyHelper<int> _diffAdditions;
        public int DiffAdditions => _diffAdditions.Value;

        private readonly ObservableAsPropertyHelper<int> _diffDeletions;
        public int DiffDeletions => _diffDeletions.Value;

        private readonly ObservableAsPropertyHelper<int> _diffModifications;
        public int DiffModifications => _diffModifications.Value;

        private bool _approved;
        public bool Approved
        {
            get { return _approved; }
            private set { this.RaiseAndSetIfChanged(ref _approved, value); }
        }

        public CommitViewModel(
            string username, string repository, Commit commit, bool showRepository = false,
            IApplicationService applicationService = null, IActionMenuService actionMenuService = null,
            IAlertDialogService alertDialogService = null)
            : this(username, repository, commit.Hash, showRepository, applicationService, 
                   actionMenuService, alertDialogService)
        {
            Commit = commit;
        }

        public CommitViewModel(
            string username, string repository, string node, bool showRepository = false,
            IApplicationService applicationService = null, IActionMenuService actionMenuService = null,
            IAlertDialogService alertDialogService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            actionMenuService = actionMenuService ?? Locator.Current.GetService<IActionMenuService>();
            alertDialogService = alertDialogService ?? Locator.Current.GetService<IAlertDialogService>();

            User = username;
            Repository = repository;
            Node = node;
            ShowRepository = showRepository;

            var shortNode = Node.Substring(0, Node.Length > 7 ? 7 : Node.Length);
            Title = $"Commit {shortNode}";

            GoToUserCommand
                .OfType<string>()
                .Select(x => new UserViewModel(x))
                .Subscribe(NavigateTo);

            GoToRepositoryCommand
                .Select(_ => new RepositoryViewModel(username, repository))
                .Subscribe(NavigateTo);

            GoToAddedFiles = ReactiveCommand.Create(
                this.WhenAnyValue(x => x.DiffAdditions).Select(x => x > 0));

            GoToRemovedFiles = ReactiveCommand.Create(
                this.WhenAnyValue(x => x.DiffDeletions).Select(x => x > 0));

            GoToModifiedFiles = ReactiveCommand.Create(
                this.WhenAnyValue(x => x.DiffModifications).Select(x => x > 0));

            var canShowMenu = this.WhenAnyValue(x => x.Commit).Select(x => x != null);

            ShowMenuCommand = ReactiveCommand.Create(canShowMenu);
            ShowMenuCommand.Subscribe(sender =>
            {
                var uri = new Uri($"https://bitbucket.org/{User}/{Repository}/commits/{Node}");
                var menu = actionMenuService.Create();
                menu.AddButton("Add Comment", AddCommentCommand);
                menu.AddButton("Copy SHA", () => actionMenuService.SendToPasteBoard(Node));
                menu.AddButton("Share", () => actionMenuService.ShareUrl(sender, uri));
                menu.AddButton("Show In Bitbucket", () => NavigateTo(new WebBrowserViewModel(uri.AbsoluteUri)));
                menu.Show(sender);
            });

            ToggleApproveButton = ReactiveCommand.CreateAsyncTask(async _ => 
            {
                if (Approved)
                    await _applicationService.Client.Commits.UnapproveCommit(User, Repository, Node);
                else
                    await _applicationService.Client.Commits.ApproveCommit(User, Repository, Node);

                Commit = await _applicationService.Client.Commits.GetCommit(User, Repository, Node);
            });

            ToggleApproveButton
                .ThrownExceptions
                .Subscribe(x => alertDialogService.Alert("Error", "Unable to approve commit: " + x.Message).ToBackground());

            var changesetFiles = 
                this.WhenAnyValue(x => x.Changeset)
                .IsNotNull()
                .SelectMany(x => x.Files ?? Enumerable.Empty<ChangesetModel.FileModel>());

            changesetFiles
                .Count(x => x.Type == ChangesetModel.FileType.Added)
                .ToProperty(this, x => DiffAdditions, out _diffAdditions);

            changesetFiles
                .Count(x => x.Type == ChangesetModel.FileType.Removed)
                .ToProperty(this, x => DiffDeletions, out _diffDeletions);

            changesetFiles
                .Count(x => x.Type == ChangesetModel.FileType.Modified)
                .ToProperty(this, x => DiffModifications, out _diffModifications);

            var commitFiles = new ReactiveList<ChangesetModel.FileModel>();
            CommitFiles = commitFiles.CreateDerivedCollection(x =>
            {
                var vm = new CommitFileItemViewModel(x.File, x.Type);
                vm.GoToCommand
                  .Select(_ => new ChangesetDiffViewModel(username, repository, node, x.File))
                  .Subscribe(NavigateTo);
                return vm;
            });

            this.WhenAnyValue(x => x.Changeset)
                .Subscribe(x => commitFiles.Reset(x?.Files ?? Enumerable.Empty<ChangesetModel.FileModel>()));

            this.WhenAnyValue(x => x.Commit).Subscribe(x => {
                var currentUsername = applicationService.Account.Username;
                Approved = x?.Participants
                    .FirstOrDefault(y => string.Equals(y.User.Username, currentUsername, StringComparison.OrdinalIgnoreCase))
                    ?.Approved ?? false;
            });

            LoadCommand = ReactiveCommand.CreateAsyncTask(_ => {
                var commit = applicationService.Client.Commits.GetCommit(User, Repository, Node)
                    .OnSuccess(x => Commit = x);
                var changeset = applicationService.Client.Commits.GetChangeset(User, Repository, Node)
                    .OnSuccess(x => Changeset = x);
     
                RetrieveAllComments().ToBackground();

                return Task.WhenAll(commit, changeset);
            });
        }

        private async Task RetrieveAllComments()
        {
            var comments = await _applicationService.Client.AllItems(x => x.Commits.GetCommitComments(User, Repository, Node));
            Comments = comments.ToList();
        }

        public async Task AddComment(string text)
        {
            var model = new CreateChangesetCommentModel { Content = text };
            await _applicationService.Client.Commits.AddComment(User, Repository, Node, model);
            await RetrieveAllComments();
        }
    }
}

