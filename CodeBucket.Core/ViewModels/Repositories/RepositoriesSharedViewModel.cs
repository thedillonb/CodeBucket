using System.Linq;
using System.Threading.Tasks;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoriesSharedViewModel : RepositoriesViewModel
    {
        public RepositoriesSharedViewModel()
        {
            ShowRepositoryOwner = true;
        }

        protected override Task Load(bool forceCacheInvalidation)
        {
            return Repositories.SimpleCollectionLoad(() => {
                var items = this.GetApplication().Client.Account.GetRepositories(forceCacheInvalidation);
                return items.Where(x => !string.Equals(x.Owner, this.GetApplication().Account.Username, System.StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Name).ToList();
            });
        }
    }
}

