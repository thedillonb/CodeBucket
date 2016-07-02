using System.Threading.Tasks;
using CodeBucket.Client.Models;
using CodeBucket.Client.Models.V2;

namespace CodeBucket.Core.ViewModels.Commits
{
	public class CommitsViewModel : BaseCommitsViewModel
    {
        public string Branch { get; private set; }

        protected override async Task<Collection<CommitModel>> GetRequest(string next)
        {
            return await (next == null ? 
                this.GetApplication().Client.Commits.GetAll(Username, Repository, Branch) :
                this.GetApplication().Client.Get<Collection<CommitModel>>(next));
        }

        public void Init(NavObject navObject)
        {
            Branch = navObject.Branch;
            base.Init(navObject);
        }

		public new class NavObject : BaseCommitsViewModel.NavObject
        {
            public string Branch { get; set; }
        }
    }
}

