using System.Threading.Tasks;
using CodeBucket.Core.ViewModels.Repositories;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Repositories
{
    class UserRepositoriesViewModel : RepositoriesViewModel
    {
        public string Username
        {
            get;
            private set;
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
        }

        protected override Task Load(bool forceCacheInvalidation)
        {
			return Repositories.SimpleCollectionLoad(() => this.GetApplication().Client.Users[Username].GetInfo(forceCacheInvalidation).Repositories.OrderBy(x => x.Name).ToList());
        }

        public class NavObject
        {
            public string Username { get; set; }
        }
    }
}
