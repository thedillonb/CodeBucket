using CodeBucket.Views.User;

namespace CodeBucket.Views.Repositories
{
    public class WatchersView : BaseUserCollectionView
    {
        public WatchersView()
            : base("There are no watchers.")
		{
			Title = "Watchers";
		}
    }
}

