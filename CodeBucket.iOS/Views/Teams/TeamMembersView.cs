using CodeBucket.iOS.Views.User;

namespace CodeBucket.iOS.Views.Teams
{
    public class TeamMembersView : BaseUserCollectionView
    {
        public override void ViewDidLoad()
        {
            Title = "Members";
            NoItemsText = "No Members".t();

            base.ViewDidLoad();
        }
    }
}