using BitbucketSharp.Models.V2;
using System.Threading.Tasks;
using CodeBucket.Core.Services;
using Splat;

namespace CodeBucket.Core.ViewModels.Commits
{
    public class CommitsViewModel : BaseCommitsViewModel
    {
        private readonly IApplicationService _applicationService;
        private readonly string _username, _repository, _branch;

        protected override Task<Collection<Commit>> GetRequest()
        {
            return _applicationService.Client.Commits.GetCommits(_username, _repository, _branch);
        }

        public CommitsViewModel(
            string username, string repository, string branch,
            IApplicationService applicationService = null)
            : base(username, repository, applicationService)
        {
            _applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            _username = username;
            _repository = repository;
            _branch = branch;
        }
    }
}

