using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using BitbucketSharp;
using BitbucketSharp.Models;
using BitbucketSharp.Models.V2;
using CodeBucket.Core.Services;
using ReactiveUI;
using Splat;

namespace CodeBucket.Core.Stores
{
    public class CommitStore : ReactiveObject
    {
        private static readonly IDictionary<object, WeakReference<CommitStore>> _stores
            = new Dictionary<object, WeakReference<CommitStore>>();

        private readonly IApplicationService _applicationService;

        public string Username { get; }

        public string Repository { get; }

        public string Node { get; }

        public ReactiveCommand<Commit> LoadCommit { get; }

        private Commit _commit;
        public Commit Commit
        {
            get { return _commit; }
            private set { this.RaiseAndSetIfChanged(ref _commit, value); }
        }

        public ReactiveCommand<ChangesetModel> LoadChangeset { get; }

        private ChangesetModel _changeset;
        public ChangesetModel Changeset
        {
            get { return _changeset; }
            private set { this.RaiseAndSetIfChanged(ref _changeset, value); }
        }

        public ReactiveCommand<ImmutableList<CommitComment>> LoadComments { get; }

        private ImmutableList<CommitComment> _comments = ImmutableList.Create<CommitComment>();
        public ImmutableList<CommitComment> Comments
        {
            get { return _comments; }
            private set { this.RaiseAndSetIfChanged(ref _comments, value); }
        }

        public static CommitStore Create(string username, string repository, string node)
        {
            var key = Tuple.Create(username, repository, node);

            WeakReference<CommitStore> weakStore;
            CommitStore store;
            if (_stores.TryGetValue(key, out weakStore) && weakStore.TryGetTarget(out store))
                return store;
            
            store = new CommitStore(username, repository, node);
            _stores[key] = new WeakReference<CommitStore>(store);
            return store;
        }

        public CommitStore(string username, string repository, string node,
                          IApplicationService applicationService = null)
        {
            Username = username;
            Repository = repository;
            Node = node;
            _applicationService = applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            LoadCommit = ReactiveCommand.CreateAsyncTask(
                t => applicationService.Client.Commits.GetCommit(username, repository, node));
            LoadCommit.Subscribe(x => Commit = x);

            LoadChangeset = ReactiveCommand.CreateAsyncTask(
                t => applicationService.Client.Commits.GetChangeset(username, repository, node));
            LoadChangeset.Subscribe(x => Changeset = x);

            LoadComments = ReactiveCommand.CreateAsyncTask(async t =>
            {
                var comments = await applicationService.Client.AllItems(
                    x => x.Commits.GetCommitComments(username, repository, node));
                return ImmutableList.CreateRange(comments);
            });
            LoadComments.Subscribe(x => Comments = x);
        }

        public async Task<CommitComment> AddComment(string text)
        {
            var model = new CreateChangesetCommentModel { Content = text };
            var comment = await _applicationService.Client.Commits.AddComment(
                Username, Repository, Node, model);
            
            var commitComment = new CommitComment
            {
                CreatedOn = comment.UtcCreatedOn,
                Content = new CommitCommentContent
                {
                    Raw = comment.Content,
                    Html = comment.ContentRendered
                },
                User = new User
                {
                    DisplayName = comment.DisplayName,
                    Username = comment.Username,
                    Links = new User.LinksModel
                    {
                        Avatar = new LinkModel
                        {
                            Href = comment.UserAvatarUrl
                        }
                    }
                }
            };

            Comments = Comments.Add(commitComment);

            return commitComment;
        }

        public async Task ToggleApprove()
        {
            var commit = Commit ?? await LoadCommit.ExecuteAsyncTask();

            var currentUsername = _applicationService.Account.Username;
            var approved = commit.Participants
                ?.FirstOrDefault(y => string.Equals(currentUsername, y?.User?.Username, StringComparison.OrdinalIgnoreCase))
                ?.Approved ?? false;
            
            if (approved)
                await _applicationService.Client.Commits.UnapproveCommit(Username, Repository, Node);
            else
                await _applicationService.Client.Commits.ApproveCommit(Username, Repository, Node);

            commit = await _applicationService.Client.Commits.GetCommit(Username, Repository, Node);
            var me = commit.Participants.FirstOrDefault(
                y => string.Equals(currentUsername, y?.User?.Username, StringComparison.OrdinalIgnoreCase));
            if (me != null)
                me.Approved = !approved;
            Commit = commit;
        }
    }
}

