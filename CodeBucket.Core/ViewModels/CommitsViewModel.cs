using System;
using CodeFramework.Core.ViewModels;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;
using BitbucketSharp.Models;

namespace CodeBucket.Core.ViewModels
{
	public abstract class CommitsViewModel : LoadableViewModel
	{
		private readonly CollectionViewModel<ChangesetModel> _commits = new CollectionViewModel<ChangesetModel>();

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

		public ICommand GoToChangesetCommand
		{
			get { return new MvxCommand<ChangesetModel>(x => ShowViewModel<ChangesetViewModel>(new ChangesetViewModel.NavObject { Username = Username, Repository = Repository, Node = x.Node })); }
		}

		public CollectionViewModel<ChangesetModel> Commits
		{
			get { return _commits; }
		}

		public void Init(NavObject navObject)
		{
			Username = navObject.Username;
			Repository = navObject.Repository;
		}

		protected override Task Load(bool forceCacheInvalidation)
		{
			return Commits.SimpleCollectionLoad(() => GetRequest(null));
		}

		protected abstract List<ChangesetModel> GetRequest(string startNode);

//		public override void Update(bool force)
//		{
//			Model = new ListModel<ChangesetModel> { Data = GetData() };
//			Model.More = () => {
//				var data = GetData(Model.Data.Last().Node);
//				if (data.Count > 1)
//				{
//					data.RemoveAt(0);
//					Model.Data.AddRange(data);
//				}
//				else
//					Model.More = null;
//
//				Render();
//			};
//		}
//
//		protected List<ChangesetModel> GetData(string startNode = null)
//		{
//			var data = Application.Client.Users[User].Repositories[Slug].Changesets.GetChangesets(RequestLimit, startNode);
//			return data.Changesets.OrderByDescending(x => x.Utctimestamp).ToList();
//		}
//
//		public override void Update(bool force)
//		{
//			Model = new ListModel<ChangesetModel> { Data = GetData() };
//			Model.More = () => {
//				var data = GetData(Model.Data.Last().Node);
//				if (data.Count > 1)
//				{
//					data.RemoveAt(0);
//					Model.Data.AddRange(data);
//				}
//				else
//					Model.More = null;
//
//				Render();
//			};
//		}
//
//		protected List<ChangesetModel> GetData(string startNode = null)
//		{
//			var data = Application.Client.Users[User].Repositories[Slug].Changesets.GetChangesets(RequestLimit, startNode);
//			return data.Changesets.OrderByDescending(x => x.Utctimestamp).ToList();
//		}

		public class NavObject
		{
			public string Username { get; set; }
			public string Repository { get; set; }
		}
	}
}

