namespace CodeBucket.Views.User
{
    public class UserFollowingsView : BaseUserCollectionView
    {
        public UserFollowingsView()
            : base("There are no followers.")
        {
            Title = "Following";
        }
    }
}

