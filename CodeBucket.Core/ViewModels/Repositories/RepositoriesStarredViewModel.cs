using System.Linq;
using System.Threading.Tasks;
using CodeBucket.Client.Models;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoriesStarredViewModel : RepositoriesViewModel
    {
        protected override async Task Load()
        {
            var items = await this.GetApplication().Client.Users.GetCurrentUserRepositoriesFollowing();
            var repos = items.Select(x =>
            {
                return new Repository
                {
                    Description = x.Description,
                    Name = x.Name,
                    FullName = x.Owner + "/" + x.Slug,
                    Owner = new User
                    {
                        Username = x.Owner
                    }
                };
            });
            Repositories.Items.Reset(repos);
        }
    }
}

