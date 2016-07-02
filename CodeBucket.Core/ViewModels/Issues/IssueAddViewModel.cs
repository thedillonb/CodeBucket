using System;
using MvvmCross.Core.ViewModels;
using System.Threading.Tasks;
using CodeBucket.Core.Messages;
using CodeBucket.Client.Models;

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

				var createIssueModel = new CreateIssueModel 
				{ 
					Title = Title, 
					Content = Content ?? string.Empty, 
					Milestone = Milestone, 
					Responsible = AssignedTo != null ? AssignedTo.Username : null,
					Component = Component,
					Version = Version,
                    Kind = Kind != null ? Kind.ToLower() : null,
                    Priority = Priority != null ? Priority.ToLower() : null,
				};

				IsSaving = true;
                var data = await this.GetApplication().Client.Issues.Create(Username, Repository, createIssueModel);
				Messenger.Publish(new IssueAddMessage(this) { Issue = data });
				ChangePresentation(new MvxClosePresentationHint(this));
			}
			catch (Exception e)
			{
                DisplayAlert("Unable to save the issue: " + e.Message).ToBackground();
			}
			finally
			{
				IsSaving = false;
			}
		}

		protected override Task Load()
		{
			return Task.FromResult(false);
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

