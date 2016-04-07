using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using CodeBucket.Core.ViewModels.User;
using MvvmCross.Plugins.Messenger;
using CodeBucket.Core.Messages;
using CodeBucket.Core.Services;
using BitbucketSharp.Models;
using System.Linq;
using System;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueViewModel : BaseViewModel, ILoadableViewModel
    {
        private readonly IApplicationService _applicationService;
        private MvxSubscriptionToken _editToken, _deleteToken;

        public int Id { get; private set; }

        public string Username { get; private set; }

        public string Repository { get; private set; }

        private string _markdownDescription;
        public string MarkdownDescription
        {
            get { return _markdownDescription; }
            private set { this.RaiseAndSetIfChanged(ref _markdownDescription, value); }
        }

		private IssueModel _issueModel;
        public IssueModel Issue
        {
            get { return _issueModel; }
            private set { this.RaiseAndSetIfChanged(ref _issueModel, value); }
        }

        public ICommand GoToAssigneeCommand { get; }

        public ICommand GoToMilestoneCommand { get; }

        public ICommand GoToEditCommand { get; }

        public ReactiveUI.IReactiveCommand LoadCommand { get; }

        public CollectionViewModel<CommentModel> Comments { get; } = new CollectionViewModel<CommentModel>();

        public ICommand GoToWeb { get; }

        public IssueViewModel(IApplicationService applicationService, IMarkdownService markdownService)
        {
            _applicationService = applicationService;

            GoToWeb = new MvxCommand<string>(x => ShowViewModel<WebBrowserViewModel>(new WebBrowserViewModel.NavObject { Url = x }));
            GoToAssigneeCommand = new MvxCommand(() => ShowViewModel<ProfileViewModel>(new ProfileViewModel.NavObject { Username = Issue.Responsible.Username }), () => Issue != null && Issue.Responsible != null);
            GoToEditCommand = new MvxCommand(() => {
                GetService<IViewModelTxService>().Add(Issue);
                ShowViewModel<IssueEditViewModel>(new IssueEditViewModel.NavObject { Username = Username, Repository = Repository, Id = Id });
            }, () => Issue != null);
            GoToMilestoneCommand = null;
            //              return new MvxCommand(() => {
            //                  if (Issue.Metadata != null && !string.IsNullOrEmpty(Issue.Metadata.Milestone))
            //                      GetService<IViewModelTxService>().Add(new IssueVersion { Name = Issue.Metadata.Milestone });
            //                  ShowViewModel<IssueMilestonesViewModel>(new IssueMilestonesViewModel.NavObject { Username = Username, Repository = Repository, Id = Id, SaveOnSelect = true });
            //              }); 

            this.Bind(x => x.Issue)
                .Subscribe(x => MarkdownDescription = x != null ? markdownService.ConvertMarkdown(x.Content) : null);

            LoadCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(async t => {
                var issueTask = applicationService.Client.Repositories.Issues.GetIssue(Username, Repository, Id);
                applicationService.Client.Repositories.Issues.GetComments(Username, Repository, Id)
                    .ToBackground(Comments.Items.Reset);
                Issue = await issueTask;
            });
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
            var comment = await _applicationService.Client.Repositories.Issues.AddComment(Username, Repository, Id, text);
            Comments.Items.Add(comment);
        }

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
			public int Id { get; set; }
        }
    }
}

