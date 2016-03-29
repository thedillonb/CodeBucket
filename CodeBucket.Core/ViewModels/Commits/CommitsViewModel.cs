using BitbucketSharp.Models.V2;
using System.Threading.Tasks;
using CodeBucket.Core.Services;

namespace CodeBucket.Core.ViewModels.Commits
{
    public class CommitsViewModel : BaseCommitsViewModel
    {
        public string Branch { get; private set; }

        public string Username { get; set; }

        public string Repository { get; set; }

        protected override Task<Collection<Commit>> GetRequest()
        {
            return this.GetApplication().Client.Repositories.GetCommits(Username, Repository, Branch);
        }

        public CommitsViewModel(IApplicationService applicationService)
            : base(applicationService)
        {
        }

        public void Init(NavObject navObject)
        {
            Branch = navObject.Branch;
            Username = navObject.Username;
            Repository = navObject.Repository;
        }

		public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
            public string Branch { get; set; }
        }
    }
}

