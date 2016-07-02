using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using System.Linq;
using CodeBucket.Core.ViewModels.Commits;

namespace CodeBucket.Core.ViewModels.Source
{
    public class ChangesetBranchesViewModel : LoadableViewModel
    {
		private readonly CollectionViewModel<ViewModel> _items = new CollectionViewModel<ViewModel>();

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

		public CollectionViewModel<ViewModel> Branches
        {
            get { return _items; }
        }

        public ICommand GoToBranchCommand
        {
			get { return new MvxCommand<ViewModel>(x => ShowViewModel<CommitsViewModel>(new CommitsViewModel.NavObject { Username = Username, Repository = Repository, Branch = x.Node })); }
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
            Repository = navObject.Repository;
        }

		protected override async Task Load()
        {
            var items = await this.GetApplication().Client.Repositories.GetBranches(Username, Repository);
            Branches.Items.Reset(items.Select(x => new ViewModel { Name = x.Key, Node = x.Value.Node }));
        }

		public class ViewModel
		{
			public string Name { get; set; }
			public string Node { get; set; }
		}

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
        }
    }
}

