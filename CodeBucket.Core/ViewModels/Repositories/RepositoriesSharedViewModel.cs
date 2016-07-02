using System.Linq;
using System.Threading.Tasks;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoriesSharedViewModel : RepositoriesViewModel
    {
        protected override Task Load()
        {
            var username = this.GetApplication().Account.Username;
            Repositories.Items.Clear();

            return this.GetApplication().Client.ForAllItems(
                x => x.Repositories.GetRepositories(username), 
                x => Repositories.Items.AddRange(x.Where(
                    y => !string.Equals(y.Owner.Username, username, System.StringComparison.OrdinalIgnoreCase))));
        }
    }
}

