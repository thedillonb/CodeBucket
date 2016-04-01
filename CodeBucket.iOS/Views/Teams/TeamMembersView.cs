using CodeBucket.Views.User;

namespace CodeBucket.Views.Teams
{
    public class TeamMembersView : BaseUserCollectionView
    {
        public TeamMembersView()
            : base("There are no members.")
        {
            Title = "Members";
        }
    }
}