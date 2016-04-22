using CodeBucket.ViewControllers.User;

namespace CodeBucket.ViewControllers.Teams
{
    public class TeamMembersViewController : BaseUserCollectionViewController
    {
        public TeamMembersViewController()
            : base("There are no members.")
        {
            Title = "Members";
        }
    }
}