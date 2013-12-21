using CodeBucket.Core.ViewModels.Groups;
using CodeBucket.iOS.Views.User;

namespace CodeBucket.iOS.Views.Groups
{
	public class GroupView : BaseUserCollectionView
    {
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			Title = ((GroupViewModel)ViewModel).GroupName;
		}

        public override void ViewDidLoad()
        {
			NoItemsText = "No Members".t();
			base.ViewDidLoad();
        }
    }
}

