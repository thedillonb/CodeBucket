using CodeBucket.Core.ViewModels.Groups;
using CodeBucket.Views.User;

namespace CodeBucket.Views.Groups
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
			NoItemsText = "No Members";
			base.ViewDidLoad();
        }
    }
}

