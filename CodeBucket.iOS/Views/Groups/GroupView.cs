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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = (ViewModel as GroupViewModel)?.Name;
        }
    }
}

