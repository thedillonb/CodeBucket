using System.Threading.Tasks;
using CodeBucket.Client.V1;
using CodeBucket.Core.Messages;
using CodeBucket.Core.Services;
using Splat;

namespace CodeBucket.Core.ViewModels.Issues
{
	public class IssueAddViewModel : IssueModifyViewModel
    {
        private readonly IApplicationService _applicationService;
        private readonly IMessageService _messageService;

        public IssueAddViewModel(
            string username, string repository,
            IApplicationService applicationService = null,
            IMessageService messageService = null)
            : base(username, repository)
        {
            _applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            _messageService = messageService ?? Locator.Current.GetService<IMessageService>();

            Title = "New Issue";
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
                Priority = Priority?.ToLower()
			};

            var issue = await _applicationService.Client.Issues.Create(Username, Repository, newIssue);
            _messageService.Send(new IssueAddMessage(issue));
		}
    }
}

