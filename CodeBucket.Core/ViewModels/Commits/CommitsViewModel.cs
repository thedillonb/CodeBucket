using BitbucketSharp.Models.V2;

namespace CodeBucket.Core.ViewModels.Commits
{
	public class CommitsViewModel : BaseCommitsViewModel
    {
        public string Branch { get; private set; }

        protected override Collection<CommitModel> GetRequest(string next)
        {
            return next == null ? 
                this.GetApplication().Client.Users[Username].Repositories[Repository].Changesets.GetCommits(Branch) : 
                this.GetApplication().Client.Request2<Collection<CommitModel>>(next);
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

