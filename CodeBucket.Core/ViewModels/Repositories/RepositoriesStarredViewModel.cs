using System.Threading.Tasks;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoriesStarredViewModel : RepositoriesViewModel
    {
        public RepositoriesStarredViewModel()
        {
            ShowRepositoryOwner = true;
        }

        protected override Task Load(bool forceCacheInvalidation)
        {
            return Repositories.SimpleCollectionLoad(() => this.GetApplication().Client.Account.GetRepositoriesFollowing(forceCacheInvalidation));
        }
    }
}

