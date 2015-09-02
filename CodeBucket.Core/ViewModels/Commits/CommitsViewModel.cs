using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;
using BitbucketSharp.Models;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Commits
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

        public string Branch
        {
            get;
            private set;
        }

		public ICommand GoToChangesetCommand
		{
			get { return new MvxCommand<ChangesetModel>(x => ShowViewModel<ChangesetViewModel>(new ChangesetViewModel.NavObject { Username = Username, Repository = Repository, Node = x.RawNode })); }
		}

		public CollectionViewModel<ChangesetModel> Commits
		{
			get { return _commits; }
		}

		public void Init(NavObject navObject)
		{
			Username = navObject.Username;
			Repository = navObject.Repository;
            Branch = navObject.Branch;
		}

		protected override Task Load(bool forceCacheInvalidation)
		{
            return Commits.RequestModel(() => GetRequest(Branch), response => {
                Commits.MoreItems = () => {
                    var items = GetRequest(Commits.Items.Last().Node);
                    if (items.Count > 1)
                    {
                        items.RemoveAt(0);
                        Commits.Items.AddRange(items);
                    }
                    else
                    {
                        Commits.MoreItems = null;
                    }
                };

                Commits.Items.Reset(response);
            });
		}

		protected abstract List<ChangesetModel> GetRequest(string startNode);

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
            public string Branch { get; set; }
		}
	}
}

