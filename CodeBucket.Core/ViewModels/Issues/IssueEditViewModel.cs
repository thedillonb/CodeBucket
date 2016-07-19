using System.Threading.Tasks;
using CodeBucket.Core.Services;
using ReactiveUI;
using CodeBucket.Client.V1;
using Splat;
using CodeBucket.Core.Messages;

namespace CodeBucket.Core.ViewModels.Issues
{
	public class IssueEditViewModel : IssueModifyViewModel
    {
        private readonly IApplicationService _applicationService;
        private readonly IMessageService _messageService;

        private string _status;
        public string Status
        {
            get { return _status; }
            set { this.RaiseAndSetIfChanged(ref _status, value); }
        }

        private Issue _issue;
		public Issue Issue
		{
			get { return _issue; }
			set 
            {
                this.RaiseAndSetIfChanged(ref _issue, value);

                Assignee.SelectedValue = _issue.Responsible?.Username;
                IssueTitle = _issue.Title;
				Content = _issue.Content;
				Status = _issue.Status;
				Priority = _issue.Priority;
				Kind = _issue.Metadata.Kind;
                Milestones.SelectedValue = _issue.Metadata.Milestone;
				Components.SelectedValue = _issue.Metadata.Component;
                Versions.SelectedValue = _issue.Metadata.Version;
			}
		}

        public IssueEditViewModel(
            string username, string repository, Issue issue,
            IApplicationService applicationService = null,
            IMessageService messageService = null)
            : this(username, repository, issue.LocalId, applicationService, messageService)
        {
            Issue = issue;
        }

        public IssueEditViewModel(
            string username, string repository, int id,
            IApplicationService applicationService = null,
            IMessageService messageService = null)
            : base(username, repository)
        {
            _applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            _messageService = messageService ?? Locator.Current.GetService<IMessageService>();

            Title = "Edit Issue";
        }

		protected override async Task Save()
		{
            var newIssue = new NewIssue
            {
                Title = IssueTitle,
                Content = Content ?? string.Empty,
                Milestone = Milestones.SelectedValue,
                Responsible = Assignee.SelectedValue,
                Component = Components.SelectedValue,
                Version = Versions.SelectedValue,
                Kind = Kind?.ToLower(),
                Status = Status?.ToLower(),
                Priority = Priority?.ToLower()
            };

            var issue = await _applicationService.Client.Issues.Edit(Username, Repository, Issue.LocalId, newIssue);
            _messageService.Send(new IssueUpdateMessage(issue));
		}
    }
}

