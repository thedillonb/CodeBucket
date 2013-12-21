using CodeBucket.iOS.Views.User;

namespace CodeBucket.iOS.Views.Repositories
{
    public class WatchersView : BaseUserCollectionView
    {
		public override void ViewDidLoad()
		{
			Title = "Watchers".t();
			NoItemsText = "No Watchers".t();
			base.ViewDidLoad();
		}
    }
}

