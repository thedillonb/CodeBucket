using System;
using Cirrious.MvvmCross.ViewModels;
using System.Threading.Tasks;
using CodeBucket.Core.Messages;
using BitbucketSharp.Models;

namespace CodeBucket.Core.ViewModels.Issues
{
	public class IssueAddViewModel : IssueModifyViewModel
    {
		protected override async Task Save()
		{
			try
			{
				if (string.IsNullOrEmpty(Title))
					throw new Exception("Issue must have a title!");

				string assignedTo = AssignedTo != null ? AssignedTo.Username : null;
				string milestone = Milestone != null ? Milestone.Name : null;
				string content = Content ?? string.Empty;

				IsSaving = true;
				var createIssueModel = new CreateIssueModel() { Title = Title, Content = content, Milestone = milestone, Responsible = assignedTo };
				var data = await Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Issues.Create(createIssueModel));
				Messenger.Publish(new IssueAddMessage(this) { Issue = data });
				ChangePresentation(new MvxClosePresentationHint(this));
			}
			catch (Exception e)
			{
				ReportError(e);
			}
			finally
			{
				IsSaving = false;
			}
		}

		public void Init(NavObject navObject)
		{
			base.Init(navObject.Username, navObject.Repository);
		}

		public class NavObject
		{
			public string Username { get; set; }
			public string Repository { get; set; }
		}
    }
}

