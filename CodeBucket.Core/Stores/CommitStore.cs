using System;
using System.Collections.Generic;
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
        private readonly IApplicationService _applicationService;
        private readonly string _username, _repository, _node;

        public ReactiveCommand<ChangesetModel> LoadChangeset { get; }

        private ChangesetModel _changeset;
        public ChangesetModel Changeset
        {
            get { return _changeset; }
            private set { this.RaiseAndSetIfChanged(ref _changeset, value); }
        }

        public ReactiveCommand<Commit> LoadCommit { get; }

        private Commit _commit;
        public Commit Commit
        {
            get { return _commit; }
            private set { this.RaiseAndSetIfChanged(ref _commit, value); }
        }

        public ReactiveCommand<List<CommitComment>> LoadComments { get; }

        public ReactiveList<CommitComment> Comments { get; } = new ReactiveList<CommitComment>();

        public CommitStore(string username, string repository, string node,
                           IApplicationService applicationService = null)
        {
            _username = username;
            _repository = repository;
            _node = node;
            _applicationService = applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            LoadChangeset = ReactiveCommand.CreateAsyncTask(_ =>
                applicationService.Client.Commits.GetChangeset(username, repository, node));
            LoadChangeset.Subscribe(x => Changeset = x);

            LoadCommit = ReactiveCommand.CreateAsyncTask(_ =>
                applicationService.Client.Commits.GetCommit(username, repository, node));
            LoadCommit.Subscribe(x => Commit = x);

            LoadComments = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var comments = await applicationService.Client.AllItems(x => x.Commits.GetCommitComments(username, repository, node));
                return comments.ToList();
            });
            LoadComments.Subscribe(Comments.Reset);
        }

        public async Task AddComment(string text)
        {
            var model = new CreateChangesetCommentModel { Content = text };
            var comment = await _applicationService.Client.Commits.AddComment(_username, _repository, _node, model);
            Comments.Add(new CommitComment
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
            });
        }

        //private static IDictionary<object, WeakReference<CommitStore>> _
        //public static CommitStore Get(string username, string repository, string node)
        //{
            
        //}
    }
}

