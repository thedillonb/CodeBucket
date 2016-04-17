using CodeBucket.Views.User;

namespace CodeBucket.Views.Repositories
{
    public class RepositoryWatchersView : BaseUserCollectionView
    {
        public RepositoryWatchersView()
            : base("There are no watchers.")
		{
			Title = "Watchers";
		}
    }
}

