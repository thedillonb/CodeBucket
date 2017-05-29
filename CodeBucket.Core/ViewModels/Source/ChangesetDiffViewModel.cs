using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using CodeBucket.Client;
using CodeBucket.Client.V1;
using CodeBucket.Core.Services;
using ReactiveUI;
using Splat;

namespace CodeBucket.Core.ViewModels.Source
{
    public class ChangesetDiffViewModel : BaseViewModel, ILoadableViewModel
    {
        private readonly IApplicationService _applicationService;
		private ChangesetFile _commitFileModel;

        public string Username { get; }

        public string Repository { get; }

        public string Node { get; }

        public string Filename { get; }

        private string _binaryFilePath;
		public string BinaryFilePath
        {
            get { return _binaryFilePath; }
            private set { this.RaiseAndSetIfChanged(ref _binaryFilePath, value); }
        }

        private List<CommitComment> _comments = new List<CommitComment>();
        public List<CommitComment> Comments
        {
            get { return _comments; }
            private set { this.RaiseAndSetIfChanged(ref _comments, value); }
        }

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        private string _changeType;
        public string ChangeType
        {
            get { return _changeType; }
            private set { this.RaiseAndSetIfChanged(ref _changeType, value); }
        }

        private List<Hunk> _patch;
        public List<Hunk> Patch
        {
            get { return _patch; }
            private set { this.RaiseAndSetIfChanged(ref _patch, value); }
        }

        public ChangesetDiffViewModel(
            string username, string repository, string branch, ChangesetFile model)
            : this(username, repository, branch, model.File)
        {
            _commitFileModel = model;
            ChangeType = model.Type.ToString();
        }

        public ChangesetDiffViewModel(
            string username, string repository, string node, string filename,
            IApplicationService applicationService = null, IDiffService diffService = null)
        {
            Username = username;
            Repository = repository;
            Node = node;
            Filename = filename;
            _applicationService = applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            diffService = diffService ?? Locator.Current.GetService<IDiffService>();

            var actualFilename = Path.GetFileName(filename);
            if (actualFilename == null)
                actualFilename = filename.Substring(filename.LastIndexOf('/') + 1);

            Title = actualFilename;

            LoadCommand = ReactiveCommand.CreateFromTask(async t =>
            {
                Patch = null;
                BinaryFilePath = null;

                var currentFilePath = Path.Combine(Path.GetTempPath(), actualFilename);
                var hasCurrent = _commitFileModel.Type != "removed";
                var hasPast = _commitFileModel.Type != "added";
                var isBinary = false;

                if (hasCurrent)
                {
                    using (var stream = new FileStream(currentFilePath, FileMode.Create, FileAccess.Write))
                    {
                        await applicationService.Client.Repositories.GetRawFile(username, repository, node, filename, stream);
                    }

                    using (var stream = new FileStream(currentFilePath, FileMode.Open, FileAccess.Read))
                    {
                        var buffer = new byte[1024];
                        var read = stream.Read(buffer, 0, 1024);
                        isBinary = buffer.Take(read).Any(x => x == 0);
                    }
                }

                if (isBinary)
                {
                    BinaryFilePath = currentFilePath;
                    return;
                }

                var parentFilePath = actualFilename + ".parent";
                var pastFilePath = Path.Combine(Path.GetTempPath(), parentFilePath);

                if (hasPast)
                {
                    var changeset = await applicationService.Client.Commits.GetChangeset(username, repository, node);
                    var parent = changeset.Parents?.FirstOrDefault();
                    if (parent == null)
                      throw new Exception("Diff has no parent. Unable to generate view.");
                    

                    using (var stream = new FileStream(pastFilePath, FileMode.Create, FileAccess.Write))
                    {
                        await applicationService.Client.Repositories.GetRawFile(username, repository, parent, filename, stream);
                    }

                    using (var stream = new FileStream(pastFilePath, FileMode.Open, FileAccess.Read))
                    {
                        var buffer = new byte[1024];
                        var read = stream.Read(buffer, 0, 1024);
                        isBinary = buffer.Take(read).Any(x => x == 0);
                    }
                }

                if (isBinary)
                {
                    BinaryFilePath = currentFilePath;
                    return;
                }

                string newText = null, oldText = null;

                if (hasCurrent)
                    newText = await Task.Run(() => File.ReadAllText(currentFilePath));

                if (hasPast)
                    oldText = await Task.Run(() => File.ReadAllText(pastFilePath));

                Patch = diffService.CreateDiff(oldText, newText, 5).ToList();

                var items = await applicationService.Client.AllItems(x => x.Commits.GetComments(username, repository, node));
                var comments = items.Where(x => x.Inline?.Path == filename).ToList();
                var commentsMap = comments.ToDictionary(x => x.Id);
                foreach (var comment in comments.Where(x => x.Parent != null))
                {
                    var parentComment = commentsMap[comment.Parent.Id];
                    while (parentComment?.Parent != null)
                        parentComment = commentsMap[parentComment.Parent.Id];
                    comment.Inline = parentComment.Inline;
                }

                Comments = comments;
            });
        }

		public async Task PostComment(string comment, int? lineFrom, int? lineTo)
		{
            var c = await _applicationService.Client.Commits.CreateComment(Username, Repository, Node, new NewChangesetComment
            {
                Content = comment,
                LineFrom = lineFrom,
                LineTo = lineTo,
                Filename = Filename
            });

            var newComment = new CommitComment
            {
                Content = new CommitCommentContent
                {
                    Html = c.ContentRendered,
                    Raw = c.Content
                },
                CreatedOn = c.UtcCreatedOn,
                Id = c.CommentId,
                UpdatedOn = c.UtcLastUpdated,
                Inline = new CommitCommentInline
                {
                    From = c.LineFrom,
                    Path = c.Filename,
                    To = c.LineTo
                },
                User = new Client.User
                {
                    Username = c.Username,
                    Links = new Client.User.UserLinks
                    {
                        Avatar = new Link(c.UserAvatarUrl)
                    }
                }
            };

            Comments = new List<CommitComment>(Comments.Concat(Enumerable.Repeat(newComment, 1)));
		}
    }
}

