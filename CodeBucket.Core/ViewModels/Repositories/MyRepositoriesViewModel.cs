using System.Linq;
using System.Threading.Tasks;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class MyRepositoriesViewModel : RepositoriesViewModel
    {
        protected override Task Load(bool forceCacheInvalidation)
        {
            return Repositories.SimpleCollectionLoad(() => this.GetApplication().Client.Account.GetRepositories(forceCacheInvalidation).OrderBy(x => x.Name).ToList());
        }
    }
}

