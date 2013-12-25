using System;
using CodeFramework.Core.ViewModels;
using System.Threading.Tasks;
using CodeFramework.Core.Services;
using Cirrious.CrossCore;
using System.Collections.Generic;
using System.Linq;
using BitbucketSharp.Models;

namespace CodeBucket.Core.ViewModels.Source
{
	public class ChangesetDiffViewModel : FileSourceViewModel
    {
		private readonly CollectionViewModel<CommentModel> _comments = new CollectionViewModel<CommentModel>();
		private ChangesetDiffModel _commitFileModel;
		private string _actualFilename;

		public string Username { get; private set; }

		public string Repository { get; private set; }

		public string Branch { get; private set; }

		public string Filename { get; private set; }

		public CollectionViewModel<CommentModel> Comments
		{
			get { return _comments; }
		}

		public void Init(NavObject navObject)
        {
			Username = navObject.Username;
			Repository = navObject.Repository;
			Branch = navObject.Branch;
			Filename = navObject.Filename;

			_actualFilename = System.IO.Path.GetFileName(Filename);
			if (_actualFilename == null)
				_actualFilename = Filename.Substring(Filename.LastIndexOf('/') + 1);

			Title = _actualFilename;

			_commitFileModel = Mvx.Resolve<IViewModelTxService>().Get() as ChangesetDiffModel;
        }

		protected override async Task Load(bool forceCacheInvalidation)
		{
			//Make sure we have this information. If not, go get it
			if (_commitFileModel == null)
			{
				var data = await Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Changesets[Branch].GetDiffs(forceCacheInvalidation));
				_commitFileModel = data.First(x => string.Equals(x.File, Filename));
			}

//			FilePath = CreatePlainContentFile(_commitFileModel, _actualFilename);
//			await Comments.SimpleCollectionLoad(() => this.GetApplication().Client.Users[User].Repositories[Repository].Changesets[Branch].Comments.GetComments(forceCacheInvalidation));
		}

		public async Task PostComment(string comment, int line)
		{
//			var c = await this.GetApplication().Client.ExecuteAsync(this.GetApplication().Client.Users[Username].Repositories[Repository].Commits[Branch].Comments.Create(comment, Filename, line));
//			Comments.Items.Add(c.Data);
			throw new NotImplementedException();
		}

		public class NavObject
		{
			public string Username { get; set; }
			public string Repository { get; set; }
			public string Branch { get; set; }
			public string Filename { get; set; }
		}
    }
}

