using CodeBucket.Views.User;

namespace CodeBucket.Views.Repositories
{
    public class WatchersView : BaseUserCollectionView
    {
        public WatchersView()
		{
			Title = "Watchers";
			NoItemsText = "No Watchers";
		}
    }
}

