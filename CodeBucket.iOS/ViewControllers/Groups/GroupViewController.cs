using CodeBucket.Core.ViewModels.Groups;
using CodeBucket.ViewControllers.User;

namespace CodeBucket.ViewControllers.Groups
{
	public class GroupViewController : BaseUserCollectionViewController
    {
        public GroupViewController()
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

