using System.Threading.Tasks;

namespace CodeBucket.Core.ViewModels.Repositories
{
    class UserRepositoriesViewModel : RepositoriesViewModel
    {
        public string Username { get; private set; }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
        }

        protected override Task Load()
        {
            Repositories.Items.Clear();
            return this.GetApplication().Client.ForAllItems(
                x => x.Repositories.GetRepositories(Username),
                Repositories.Items.AddRange);
        }

        public class NavObject
        {
            public string Username { get; set; }
        }
    }
}
