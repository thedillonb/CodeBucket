using System;
using System.Threading.Tasks;
using MvvmCross.Platform;
using System.Collections.Generic;
using System.Linq;
using BitbucketSharp.Models;
using CodeBucket.Core.Services;
using System.IO;

namespace CodeBucket.Core.ViewModels.Source
{
	public class ChangesetDiffViewModel : FileSourceViewModel
    {
		private readonly CollectionViewModel<ChangesetCommentModel> _comments = new CollectionViewModel<ChangesetCommentModel>();
		private ChangesetDiffModel _commitFileModel;
		private string _actualFilename;

		public string File1 { get; private set; }

		public string File2 { get; private set; }

		public string Username { get; private set; }

		public string Repository { get; private set; }

		public string Branch { get; private set; }

		public string Filename { get; private set; }

		public CollectionViewModel<ChangesetCommentModel> Comments
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
//			if (_commitFileModel == null)
//			{
//				var data = await Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Changesets[Branch].GetDiffs(forceCacheInvalidation));
//				_commitFileModel = data.First(x => string.Equals(x.File, Filename));
//			}
//
//			if (_commitFileModel.Type == "added" || _commitFileModel.Type == "modified")
//			{
//				var filepath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetFileName(Filename));
//                var content = await Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Branches[Branch].Source.GetFile(Filename));
//                var isText = content.Encoding == null;
//
//				using (var stream = new System.IO.FileStream(filepath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
//				{
//                    if (isText)
//                    {
//                        using (var s = new StreamWriter(stream))
//                            await s.WriteAsync(content.Data);
//                    }
//                    else
//                    {
//                        var data = Convert.FromBase64String(content.Data);
//                        await stream.WriteAsync(data, 0, data.Length);
//                    }
//				}
//
//				if (!isText)
//				{
//					FilePath = filepath;
//					return;
//				}
//
//				File1 = filepath;
//			}
//
//			if (_commitFileModel.Type == "removed" || _commitFileModel.Type == "modified")
//			{
//                var changeset = await Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Changesets[Branch].GetCommit());
//                if (changeset.Parents == null || changeset.Parents.Count == 0)
//					throw new Exception("Diff has no parent. Unable to generate view.");
//
//                var parent = changeset.Parents[0].Hash;
//				var filepath2 = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetFileName(Filename) + ".parent");
//                var content = await Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Branches[parent].Source.GetFile(Filename));
//                var isText = content.Encoding == null;
//
//				using (var stream = new System.IO.FileStream(filepath2, System.IO.FileMode.Create, System.IO.FileAccess.Write))
//				{
//                    if (isText)
//                    {
//                        using (var s = new StreamWriter(stream))
//                            await s.WriteAsync(content.Data);
//                    }
//                    else
//                    {
//                        var data = Convert.FromBase64String(content.Data);
//                        await stream.WriteAsync(data, 0, data.Length);
//                    }
//				}
//
//				if (!isText)
//				{
//					FilePath = filepath2;
//					return;
//				}
//
//				File2 = filepath2;
//			}
//
//			if (File1 != null)
//				FilePath = File1;
//			else if (File2 != null)
//				FilePath = File2;
//
//			Comments.SimpleCollectionLoad(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Changesets[Branch].Comments.GetComments(forceCacheInvalidation)).FireAndForget();
		}

		public async Task PostComment(string comment, int? lineFrom, int? lineTo)
		{
//			var c = await Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Changesets[Branch].Comments.Create(comment, lineFrom, lineTo, filename: Filename));
//			Comments.Items.Add(c);
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

