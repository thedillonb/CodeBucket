using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using CodeFramework.Core.ViewModels;
using CodeBucket.Core.Services;
using CodeBucket.Core.ViewModels.Repositories;
using System.Threading.Tasks;
using CodeBucket.Core.ViewModels.Source;
using BitbucketSharp.Models;
using System.Linq;
using System.Collections.Generic;

namespace CodeBucket.Core.ViewModels
{
    public class ChangesetViewModel : LoadableViewModel
    {
		private readonly CollectionViewModel<ChangesetCommentModel> _comments = new CollectionViewModel<ChangesetCommentModel>();
        private readonly IApplicationService _application;
		private List<ChangesetDiffModel> _commitModel;

		public string Node { get; private set; }

		public string User { get; private set; }

		public string Repository { get; private set; }

        public bool ShowRepository { get; private set; }

		public List<ChangesetDiffModel> Changeset
        {
            get { return _commitModel; }
            private set
            {
                _commitModel = value;
                RaisePropertyChanged(() => Changeset);
            }
        }

        public ICommand GoToRepositoryCommand
        {
            get { return new MvxCommand(() => ShowViewModel<RepositoryViewModel>(new RepositoryViewModel.NavObject { Username = User, Repository = Repository })); }
        }

		public ICommand GoToFileCommand
		{
			get
			{ 
				return new MvxCommand<ChangesetDiffModel>(x =>
				{
//						if (x. == null)
//						{
//							ShowViewModel<SourceViewModel>(new SourceViewModel.NavObject { GitUrl = x.ContentsUrl, HtmlUrl = x.BlobUrl, Name = x.Filename, Path = x.Filename, ForceBinary = true });
//						}
//						else
//						{
						Cirrious.CrossCore.Mvx.Resolve<CodeFramework.Core.Services.IViewModelTxService>().Add(x);
						ShowViewModel<ChangesetDiffViewModel>(new ChangesetDiffViewModel.NavObject { Username = User, Repository = Repository, Branch = Node, Filename = x.File });
//						}

				});
			}
		}

		public CollectionViewModel<ChangesetCommentModel> Comments
        {
            get { return _comments; }
        }

        public ChangesetViewModel(IApplicationService application)
        {
            _application = application;
        }

        public void Init(NavObject navObject)
        {
            User = navObject.Username;
            Repository = navObject.Repository;
            Node = navObject.Node;
            ShowRepository = navObject.ShowRepository;
        }

        protected override Task Load(bool forceCacheInvalidation)
        {
			var t1 = this.RequestModel(() => this.GetApplication().Client.Users[User].Repositories[Repository].Changesets[Node].GetDiffs(forceCacheInvalidation), response => Changeset = response);
			Comments.SimpleCollectionLoad(() => this.GetApplication().Client.Users[User].Repositories[Repository].Changesets[Node].Comments.GetComments(forceCacheInvalidation)).FireAndForget();
            return t1;
        }

        public async Task AddComment(string text)
        {
			var c = await Task.Run(() => this.GetApplication().Client.Users[User].Repositories[Repository].Changesets[Node].Comments.Create(text));
            Comments.Items.Add(c);
        }

		public async void Approve()
		{
//			Application.Client.Users[User].Repositories[Slug].Changesets[Node].Approve();
//			Model.Likes = Application.Client.Users[User].Repositories[Slug].Changesets[Node].GetParticipants(true);
//			Render();
		}

		public async void Unapprove()
		{
//			Application.Client.Users[User].Repositories[Slug].Changesets[Node].Unapprove();
//			Model.Likes = Application.Client.Users[User].Repositories[Slug].Changesets[Node].GetParticipants(true);
//			Render();
		}

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
            public string Node { get; set; }
            public bool ShowRepository { get; set; }
        }
    }
}

