using CodeBucket.Views.User;

namespace CodeBucket.Views.Teams
{
    public class TeamMembersView : BaseUserCollectionView
    {
        public override void ViewDidLoad()
        {
            Title = "Members";
            NoItemsText = "No Members";

            base.ViewDidLoad();
        }
    }
}