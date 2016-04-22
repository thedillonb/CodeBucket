using CodeBucket.ViewControllers.User;

namespace CodeBucket.ViewControllers.Repositories
{
    public class RepositoryWatchersViewController : BaseUserCollectionViewController
    {
        public RepositoryWatchersViewController()
            : base("There are no watchers.")
		{
			Title = "Watchers";
		}
    }
}

