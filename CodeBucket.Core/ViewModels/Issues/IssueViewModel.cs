using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using CodeBucket.Core.ViewModels.User;
using MvvmCross.Plugins.Messenger;
using CodeBucket.Core.Messages;
using CodeBucket.Core.Services;
using BitbucketSharp.Models;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueViewModel : LoadableViewModel
    {
        private MvxSubscriptionToken _editToken, _deleteToken;

		public int Id 
        { 
            get; 
            private set; 
        }

        public string Username 
        { 
            get; 
            private set; 
        }

        public string Repository 
        { 
            get; 
            private set; 
        }

		public string MarkdownDescription
		{
			get
			{
				if (Issue == null)
					return string.Empty;
				return (GetService<IMarkdownService>().ConvertMarkdown(Issue.Content));
			}
		}

		private IssueModel _issueModel;
        public IssueModel Issue
        {
            get { return _issueModel; }
            set
            {
                _issueModel = value;
                RaisePropertyChanged(() => Issue);
            }
        }

		public ICommand GoToAssigneeCommand
		{
			get { return new MvxCommand(() => ShowViewModel<ProfileViewModel>(new ProfileViewModel.NavObject { Username = Issue.Responsible.Username }), () => Issue != null && Issue.Responsible != null); }
		}


		public ICommand GoToMilestoneCommand
		{
			get 
			{ 
//				return new MvxCommand(() => {
//					if (Issue.Metadata != null && !string.IsNullOrEmpty(Issue.Metadata.Milestone))
//						GetService<IViewModelTxService>().Add(new IssueVersion { Name = Issue.Metadata.Milestone });
//					ShowViewModel<IssueMilestonesViewModel>(new IssueMilestonesViewModel.NavObject { Username = Username, Repository = Repository, Id = Id, SaveOnSelect = true });
//				}); 
                return null;
			}
		}

		public ICommand GoToEditCommand
		{
			get 
			{ 
				return new MvxCommand(() => {
					GetService<IViewModelTxService>().Add(Issue);
					ShowViewModel<IssueEditViewModel>(new IssueEditViewModel.NavObject { Username = Username, Repository = Repository, Id = Id });
				}, () => Issue != null); 
			}
		}

		private readonly CollectionViewModel<CommentModel> _comments = new CollectionViewModel<CommentModel>();
		public CollectionViewModel<CommentModel> Comments
        {
            get { return _comments; }
        }

		public ICommand GoToWeb
		{
			get { return new MvxCommand<string>(x => ShowViewModel<WebBrowserViewModel>(new WebBrowserViewModel.NavObject { Url = x })); }
		}

        protected override async Task Load(bool forceCacheInvalidation)
        {
//			var t1 = this.RequestModel(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Issues[Id].GetIssue(forceCacheInvalidation), response => Issue = response);
//
//			this.RequestModel(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Issues[Id].Comments.GetComments(forceCacheInvalidation), response => {
//                Comments.Items.Reset(response);
//			}).FireAndForget();
//
//            return t1;
        }

        public string ConvertToMarkdown(string str)
        {
			return (GetService<IMarkdownService>().ConvertMarkdown(str));
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
            Repository = navObject.Repository;
            Id = navObject.Id;
			Comments.SortingFunction = x => x.OrderBy(y => y.UtcCreatedOn);

			_editToken = Messenger.SubscribeOnMainThread<IssueEditMessage>(x =>
			{
                if (x.Issue == null || x.Issue.LocalId != Issue.LocalId)
					return;
				Issue = x.Issue;
			});

            _deleteToken = Messenger.SubscribeOnMainThread<IssueDeleteMessage>(x => ChangePresentation(new MvxClosePresentationHint(this)));
        }

        public async Task AddComment(string text)
        {
//			var comment = await Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Issues[Id].Comments.Create(text));
//			Comments.Items.Add(comment);
        }

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
			public int Id { get; set; }
        }
    }
}

