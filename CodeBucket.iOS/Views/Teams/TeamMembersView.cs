using CodeBucket.Views.User;

namespace CodeBucket.Views.Teams
{
    public class TeamMembersView : BaseUserCollectionView
    {
        public TeamMembersView()
        {
            Title = "Members";
            NoItemsText = "No Members";
        }
    }
}