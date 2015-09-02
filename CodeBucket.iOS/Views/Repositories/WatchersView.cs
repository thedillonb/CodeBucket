using CodeBucket.Views.User;

namespace CodeBucket.Views.Repositories
{
    public class WatchersView : BaseUserCollectionView
    {
		public override void ViewDidLoad()
		{
			Title = "Watchers";
			NoItemsText = "No Watchers";
			base.ViewDidLoad();
		}
    }
}

