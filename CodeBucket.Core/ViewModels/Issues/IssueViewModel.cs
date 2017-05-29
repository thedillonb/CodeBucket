using System.Threading.Tasks;
using CodeBucket.Core.ViewModels.Users;
using CodeBucket.Core.Services;
using System;
using ReactiveUI;
using System.Reactive;
using Splat;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels.Comments;
using Humanizer;
using System.Linq;
using CodeBucket.Client.V1;
using CodeBucket.Core.Messages;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueViewModel : BaseViewModel, ILoadableViewModel
    {
        private readonly IDisposable _issueMessageBus;
        private readonly ReactiveList<IssueComment> _comments = new ReactiveList<IssueComment>();
        private readonly IApplicationService _applicationService;

        public string Username { get; }

        public string Repository { get; }

        public int IssueId { get; }

        private Issue _issueModel;
        public Issue Issue
        {
            get { return _issueModel; }
            private set { this.RaiseAndSetIfChanged(ref _issueModel, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _showDescription;
        public bool ShowDescription => _showDescription.Value;

        private readonly ObservableAsPropertyHelper<string> _description;
        public string Description => _description.Value;

        private readonly ObservableAsPropertyHelper<string> _assigned;
        public string Assigned => _assigned.Value;

        public ReactiveCommand<Unit, Unit> GoToAssigneeCommand { get; }

        public ReactiveCommand<Unit, Unit> GoToEditCommand { get; }

        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        public ReactiveCommand<Unit, Unit> ShowMenuCommand { get; }

        public ReactiveCommand<Unit, Unit> DismissCommand { get; } = ReactiveCommandFactory.Empty();

        public IReadOnlyReactiveList<CommentItemViewModel> Comments { get; }

        public ReactiveCommand<string, Unit> GoToWebCommand { get; }

        public ReactiveCommand<Unit, Unit> GoToReporterCommand { get; } = ReactiveCommandFactory.Empty();

        public NewCommentViewModel NewCommentViewModel { get; }

        public IssueViewModel(
            string username, string repository, Issue issue,
            IApplicationService applicationService = null, IMarkdownService markdownService = null, 
            IMessageService messageService = null)
            : this(username, repository, issue.LocalId, applicationService, markdownService, messageService)
        {
            Issue = issue;
        }

        public IssueViewModel(
            string username, string repository, int issueId,
            IApplicationService applicationService = null, IMarkdownService markdownService = null, 
            IMessageService messageService = null, IAlertDialogService alertDialogService = null,
            IActionMenuService actionMenuService = null)
        {
            _applicationService = applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            messageService = messageService ?? Locator.Current.GetService<IMessageService>();
            markdownService = markdownService ?? Locator.Current.GetService<IMarkdownService>();
            alertDialogService = alertDialogService ?? Locator.Current.GetService<IAlertDialogService>();
            actionMenuService = actionMenuService ?? Locator.Current.GetService<IActionMenuService>();

            Title = "Issue #" + issueId;
            Username = username;
            Repository = repository;
            IssueId = issueId;

            GoToWebCommand = ReactiveCommand.Create<string>(
                url => NavigateTo(new WebBrowserViewModel(url)));

            GoToReporterCommand = ReactiveCommand.Create(
                () => NavigateTo(new UserViewModel(Issue.ReportedBy.Username)),
                this.WhenAnyValue(x => x.Issue.ReportedBy.Username).Select(x => x != null));
            
            GoToAssigneeCommand = ReactiveCommand.Create(
                () => NavigateTo(new UserViewModel(Issue.Responsible.Username)),
                this.WhenAnyValue(x => x.Issue.Responsible.Username).Select(x => x != null));
            
            GoToEditCommand = ReactiveCommand.Create(
                () => NavigateTo(new IssueEditViewModel(username, repository, Issue)),
                this.WhenAnyValue(x => x.Issue).Select(x => x != null));

            this.WhenAnyValue(x => x.Issue)
                .Select(x => x?.Responsible?.Username ?? "Unassigned")
                .ToProperty(this, x => x.Assigned, out _assigned, "Unassigned");

            this.WhenAnyValue(x => x.Issue.Content)
                .Select(x => string.IsNullOrEmpty(x) ? null : markdownService.ConvertMarkdown(x))
                .ToProperty(this, x => x.Description, out _description);

            this.WhenAnyValue(x => x.Description)
                .Select(x => !string.IsNullOrEmpty(x))
                .ToProperty(this, x => x.ShowDescription, out _showDescription);

            Comments = _comments.CreateDerivedCollection(x =>
                new CommentItemViewModel(x.AuthorInfo.Username,
                    new Utils.Avatar(x.AuthorInfo.Avatar),
                    x.UtcCreatedOn.Humanize(),
                    markdownService.ConvertMarkdown(x.Content)));

            LoadCommand = ReactiveCommand.CreateFromTask(async t => {
                var issueTask = applicationService.Client.Issues.Get(username, repository, issueId);
                applicationService.Client.Issues.GetComments(username, repository, issueId)
                                  .ToBackground(x => _comments.Reset(x.Where(y => !string.IsNullOrEmpty(y.Content))));
                Issue = await issueTask;
            });

            DeleteCommand = ReactiveCommand.CreateFromTask(async _ =>
            {
                try
                {
                    var prompt = await alertDialogService.PromptYesNo(
                        "Are you sure?", "You are about to permanently delete issue #" + Issue.LocalId + ".");

                    if (prompt)
                    {
                        await applicationService.Client.Issues.Delete(username, repository, Issue.LocalId);
                        messageService.Send(new IssueDeleteMessage(Issue));
                        DismissCommand.ExecuteNow();
                    }
                }
                catch (Exception e)
                {
                    this.Log().ErrorException("Error deleting issue", e);
                }
            });

            ShowMenuCommand = ReactiveCommand.CreateFromTask(sender =>
            {
                var menu = actionMenuService.Create();
                menu.AddButton("Edit", _ => GoToEditCommand.ExecuteNow());
                menu.AddButton("Delete", _ => DeleteCommand.ExecuteNow());
                return menu.Show(sender);
            });

            _issueMessageBus = messageService.Listen<IssueUpdateMessage>(x =>
            {
                if (x.Issue.LocalId == issueId)
                    Issue = x.Issue;
            });
        }

        public async Task AddComment(string text)
        {
            var comment = await _applicationService.Client.Issues.CreateComment(
                Username, Repository, IssueId, text);
            _comments.Add(comment);
        }
    }
}

