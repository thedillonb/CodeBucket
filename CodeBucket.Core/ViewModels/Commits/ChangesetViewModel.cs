using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using CodeBucket.Core.ViewModels.Repositories;
using System.Threading.Tasks;
using CodeBucket.Core.ViewModels.Source;
using BitbucketSharp.Models;
using System.Collections.Generic;
using System;
using CodeBucket.Core.ViewModels.User;
using CodeBucket.Core.Services;

namespace CodeBucket.Core.ViewModels.Commits
{
    public class ChangesetViewModel : LoadableViewModel
    {
		private string _patch;

		public string Node { get; private set; }

		public string User { get; private set; }

		public string Repository { get; private set; }

        public bool ShowRepository { get; private set; }

		private List<ChangesetDiffModel> _commitModel;
		public List<ChangesetDiffModel> Commits
        {
            get { return _commitModel; }
            private set
            {
                _commitModel = value;
                RaisePropertyChanged(() => Commits);
            }
        }

		private ChangesetModel _changeset;
		public ChangesetModel Changeset
		{
			get { return _changeset; }
			private set {
				_changeset = value;
				RaisePropertyChanged(() => Changeset);
			}
		}

		private readonly CollectionViewModel<ChangesetParticipantsModel> _participants = new CollectionViewModel<ChangesetParticipantsModel>();
		public CollectionViewModel<ChangesetParticipantsModel> Participants
		{
			get { return _participants; }
		}

		public ICommand GoToUserCommand
		{
			get { return new MvxCommand<string>(x => ShowViewModel<ProfileViewModel>(new ProfileViewModel.NavObject { Username = x })); }
		}

        public ICommand GoToRepositoryCommand
        {
            get { return new MvxCommand(() => ShowViewModel<RepositoryViewModel>(new RepositoryViewModel.NavObject { Username = User, RepositorySlug = Repository })); }
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
						Cirrious.CrossCore.Mvx.Resolve<IViewModelTxService>().Add(x);
						ShowViewModel<ChangesetDiffViewModel>(new ChangesetDiffViewModel.NavObject { Username = User, Repository = Repository, Branch = Node, Filename = x.File });
//						}

				});
			}
		}

		private readonly CollectionViewModel<ChangesetCommentModel> _comments = new CollectionViewModel<ChangesetCommentModel>();
		public CollectionViewModel<ChangesetCommentModel> Comments
        {
            get { return _comments; }
        }

        public void Init(NavObject navObject)
        {
            User = navObject.Username;
            Repository = navObject.Repository;
            Node = navObject.Node;
            ShowRepository = navObject.ShowRepository;
        }

		protected override async Task Load(bool forceCacheInvalidation)
        {
			var t1 = this.RequestModel(() => this.GetApplication().Client.Users[User].Repositories[Repository].Changesets[Node].GetDiffs(forceCacheInvalidation), response => Commits = response);
			var t2 = this.RequestModel(() => this.GetApplication().Client.Users[User].Repositories[Repository].Changesets[Node].GetInfo(forceCacheInvalidation), response => Changeset = response);
			await Task.WhenAll(t1, t2);
			Comments.SimpleCollectionLoad(() => this.GetApplication().Client.Users[User].Repositories[Repository].Changesets[Node].Comments.GetComments(forceCacheInvalidation)).FireAndForget();
			Participants.SimpleCollectionLoad(() => this.GetApplication().Client.Users[User].Repositories[Repository].Changesets[Changeset.RawNode].GetParticipants(forceCacheInvalidation)).FireAndForget();
        }

        public async Task AddComment(string text)
        {
			try
			{
				var c = await Task.Run(() => this.GetApplication().Client.Users[User].Repositories[Repository].Changesets[Node].Comments.Create(text));
	            Comments.Items.Add(c);
			}
			catch (Exception e)
			{
                DisplayAlert("Unable to add comment: " + e.Message);
			}
        }

		public async Task Approve()
		{
			try
			{
                await Task.Run(() => this.GetApplication().Client.Users[User].Repositories[Repository].Changesets[Node].Approve());
                await Participants.SimpleCollectionLoad(() => this.GetApplication().Client.Users[User].Repositories[Repository].Changesets[Changeset.RawNode].GetParticipants(true));
			}
			catch (Exception e)
			{
                DisplayAlert("Unable to approve commit: " + e.Message);
			}
		}

		public async Task Unapprove()
		{
			try
			{
                await Task.Run(() => this.GetApplication().Client.Users[User].Repositories[Repository].Changesets[Node].Unapprove());
                await Participants.SimpleCollectionLoad(() => this.GetApplication().Client.Users[User].Repositories[Repository].Changesets[Changeset.RawNode].GetParticipants(true));
			}
			catch (Exception e)
			{
                DisplayAlert("Unable to unapprove commit: " + e.Message);
			}
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

