using System.Threading.Tasks;
using BitbucketSharp.Models;
using System;
using Cirrious.MvvmCross.ViewModels;
using CodeBucket.Core.Messages;
using System.Windows.Input;

namespace CodeBucket.Core.ViewModels.Issues
{
	public class IssueEditViewModel : IssueModifyViewModel
    {
        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged(() => Status);
            }
        }

        private IssueModel _issue;
		public IssueModel Issue
		{
			get { return _issue; }
			set {
				_issue = value;
				RaisePropertyChanged(() => Issue);

				AssignedTo = _issue.Responsible;
				Title = _issue.Title;
				Content = _issue.Content;
				Status = _issue.Status;
				Priority = _issue.Priority;
				Kind = _issue.Metadata.Kind;
				Milestone = _issue.Metadata.Milestone;
				Component = _issue.Metadata.Component;
				Version = _issue.Metadata.Version;
			}
		}

		public int Id { get; private set; }

        public ICommand DeleteCommand
        {
            get 
            { 
                return new MvxCommand(() => PromptDelete()); 
            }
        }

        private async Task PromptDelete()
        {
            try
            {
                var alert = GetService<CodeFramework.Core.Services.IAlertDialogService>();
                if (await alert.PromptYesNo("Are you sure?", "You are about to permanently delete issue #" + Issue.LocalId + "."))
                    await Delete();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error deleting issue: " + e.Message);
            }
        }

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
                    Responsible = AssignedTo != null ? AssignedTo.Username : string.Empty,
                    Milestone = Milestone ?? string.Empty,
                    Component = Component ?? string.Empty,
                    Version = Version ?? string.Empty,
                    Kind = Kind.ToLower(),
                    Status = Status.ToLower(),
                    Priority = Priority.ToLower(),
                };

                IsSaving = true;
                var data = await Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Issues[Issue.LocalId].Update(createIssueModel));
                Messenger.Publish(new IssueEditMessage(this) { Issue = data });
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

        private async Task Delete()
        {
            try
            {
                IsSaving = true;
                await Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Issues[Issue.LocalId].Delete());
                Messenger.Publish(new IssueDeleteMessage(this) { Issue = Issue });
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

		protected override Task Load(bool forceCacheInvalidation)
		{
			if (forceCacheInvalidation || Issue == null)
				return this.RequestModel(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Issues[Id].GetIssue(forceCacheInvalidation), response => Issue = response);
			return Task.FromResult(false);
		}

		public void Init(NavObject navObject)
		{
			base.Init(navObject.Username, navObject.Repository);
			Id = navObject.Id;
			Issue = GetService<CodeFramework.Core.Services.IViewModelTxService>().Get() as IssueModel;
		}

		public class NavObject
		{
			public string Username { get; set; }
			public string Repository { get; set; }
			public int Id { get; set; }
		}
    }
}

