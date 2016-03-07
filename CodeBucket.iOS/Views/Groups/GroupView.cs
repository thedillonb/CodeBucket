using CodeBucket.Core.ViewModels.Groups;
using CodeBucket.Views.User;

namespace CodeBucket.Views.Groups
{
	public class GroupView : BaseUserCollectionView
    {
        public GroupView()
            : base("There are no members.")
        {
        }

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			Title = ((GroupViewModel)ViewModel).GroupName;
		}
    }
}

