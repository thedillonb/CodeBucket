using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using CodeBucket.Client.Models;
using CodeBucket.Core.Services;

namespace CodeBucket.Core.ViewModels.PullRequests
{
    public class PullRequestViewModel : LoadableViewModel
    {
        private readonly IMarkdownService _markdownService;
        private PullRequestModel _model;
        private bool _merged;
		private readonly CollectionViewModel<PullRequestCommentModel> _comments = new CollectionViewModel<PullRequestCommentModel>();

        public string Username { get; private set; }

        public string Repository { get; private set; }

        public int Id { get; private set; }

        public bool Merged
        {
            get { return _merged; }
            set { _merged = value; RaisePropertyChanged(() => Merged); }
        }

        public string Description { get; private set; }

        public PullRequestModel PullRequest 
        { 
            get { return _model; }
            set
            {
                _model = value;
                Description = string.IsNullOrWhiteSpace(value.Description) ? null : _markdownService.ConvertMarkdown(value.Description);
                _merged = string.Equals(value.State, "MERGED");
                RaisePropertyChanged(() => PullRequest);
            }
        }

		public CollectionViewModel<PullRequestCommentModel> Comments
        {
            get { return _comments; }
        }

		public ICommand GoToCommitsCommand
		{
			get 
            { 
                return new MvxCommand(() => {
                    if (PullRequest?.Source?.Repository == null)
                    {
                        DisplayAlert("The author has deleted the source repository for this pull request.");
                    }
                    else
                    {
                        ShowViewModel<PullRequestCommitsViewModel>(new PullRequestCommitsViewModel.NavObject { Username = Username, Repository = Repository, PullRequestId = Id });
                    }
                }); 
            }
		}

        public PullRequestViewModel(IMarkdownService markdownService)
        {
            _markdownService = markdownService;
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
            Repository = navObject.Repository;
            Id = navObject.Id;
        }

        protected override Task Load()
        {
            var t1 = this.GetApplication().Client.PullRequests.Get(Username, Repository, Id)
                         .OnSuccess(response => PullRequest = response);

            Comments.Items.Clear();
            this.GetApplication().Client
                .ForAllItems(x => x.PullRequests.GetComments(Username, Repository, Id), Comments.Items.AddRange)
                .ToBackground();

            return t1;
        }

        public async Task AddComment(string text)
        {
            await this.GetApplication().Client.PullRequests.AddComment(Username, Repository, Id, text);

            Comments.Items.Clear();
            await this.GetApplication().Client
                      .ForAllItems(x => x.PullRequests.GetComments(Username, Repository, Id), Comments.Items.AddRange);
        }

        public async Task Merge()
        {
            await this.GetApplication().Client.PullRequests.Merge(Username, Repository, Id);
            PullRequest = await this.GetApplication().Client.PullRequests.Get(Username, Repository, Id);
        }

        public ICommand MergeCommand
        {
            get { return new MvxCommand(() => Merge(), CanMerge); }
        }

        private bool CanMerge()
        {
            if (PullRequest == null)
                return false;
			return string.Equals(PullRequest.State, "open");
        }

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
            public int Id { get; set; }
        }
    }
}
