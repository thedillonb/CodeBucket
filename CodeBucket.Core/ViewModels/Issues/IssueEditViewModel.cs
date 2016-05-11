using System.Threading.Tasks;
using BitbucketSharp.Models;
using System;
using System.Windows.Input;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive;

namespace CodeBucket.Core.ViewModels.Issues
{
	public class IssueEditViewModel : IssueModifyViewModel
    {
        private string _status;
        public string Status
        {
            get { return _status; }
            set { this.RaiseAndSetIfChanged(ref _status, value); }
        }

        private IssueModel _issue;
		public IssueModel Issue
		{
			get { return _issue; }
			set 
            {
                this.RaiseAndSetIfChanged(ref _issue, value);

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

        public IReactiveCommand<Unit> DeleteCommand { get; }

        public IssueEditViewModel(
            string username, string repository, IssueModel issue,
            IAlertDialogService alertDialogService = null, IApplicationService applicationService = null)
            : this(username, repository, issue.LocalId, alertDialogService, applicationService)
        {
            Issue = issue;
        }

        public IssueEditViewModel(
            string username, string repository, int id,
            IAlertDialogService alertDialogService = null, IApplicationService applicationService = null)
            : base(username, repository, applicationService)
        {
            Title = "Edit Issue";

            DeleteCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                try
                {
                    var prompt = alertDialogService.PromptYesNo(
                        "Are you sure?", "You are about to permanently delete issue #" + Issue.LocalId + ".");
                    
                    if (await prompt)
                        await Delete();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error deleting issue: " + e.Message);
                }
            });
        }

		protected override async Task Save()
		{
//            try
//            {
//                if (string.IsNullOrEmpty(Title))
//                    throw new Exception("Issue must have a title!");
//
//                var createIssueModel = new CreateIssueModel 
//                { 
//                    Title = Title, 
//                    Content = Content ?? string.Empty, 
//                    Responsible = AssignedTo != null ? AssignedTo.Username : string.Empty,
//                    Milestone = Milestone ?? string.Empty,
//                    Component = Component ?? string.Empty,
//                    Version = Version ?? string.Empty,
//                    Kind = Kind.ToLower(),
//                    Status = Status.ToLower(),
//                    Priority = Priority.ToLower(),
//                };
//
//                IsSaving = true;
//                var data = await Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Issues[Issue.LocalId].Update(createIssueModel));
//                Messenger.Publish(new IssueEditMessage(this) { Issue = data });
//                ChangePresentation(new MvxClosePresentationHint(this));
//            }
//            catch (Exception e)
//            {
//                DisplayAlert("Unable to save the issue: " + e.Message);
//            }
//            finally
//            {
//                IsSaving = false;
//            }
		}

        private async Task Delete()
        {
//            try
//            {
//                IsSaving = true;
//                await Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Issues[Issue.LocalId].Delete());
//                Messenger.Publish(new IssueDeleteMessage(this) { Issue = Issue });
//            }
//            catch (Exception e)
//            {
//                DisplayAlert("Unable to delete issue: " + e.Message);
//            }
//            finally
//            {
//                IsSaving = false;
//            }
        }

//		protected override Task Load()
//		{
////			if (forceCacheInvalidation || Issue == null)
////				return this.RequestModel(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Issues[Id].GetIssue(forceCacheInvalidation), response => Issue = response);
//			return Task.FromResult(false);
//		}
    }
}

