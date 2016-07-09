using ReactiveUI;
using CodeBucket.Core.Services;
using System.Threading.Tasks;
using Splat;
using CodeBucket.Client;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoriesStarredViewModel : RepositoriesViewModel
    {
        public RepositoriesStarredViewModel(
            IApplicationService applicationService = null)
            : base(applicationService)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            Title = "Watched";
        }

        protected override async Task Load(IApplicationService applicationService, IReactiveList<Repository> repositories)
        {
            var watchers = await applicationService.Client.Repositories.GetWatched();
            repositories.AddRange(watchers.Select(x =>
            {
                return new Repository
                {
                    Name = x.Name,
                    CreatedOn = x.UtcCreatedOn,
                    Description = x.Description,
                    FullName = x.Owner + "/" + x.Slug,
                    UpdatedOn = x.UtcLastUpdated,
                    Owner = new User
                    {
                        Username = x.Owner,
                        Links = new User.UserLinks
                        {
                            Avatar = new Link(x.Logo)
                        }
                    }
                };
            }));
        }
    }
}

