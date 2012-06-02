namespace BitbucketSharp.Models
{
    public class InvitationModel
    {
        public string SentOn { get; set; }
        public string Permission { get; set; }
        public UserModel InvitedBy { get; set; }
        public RepositoryDetailedModel Repository { get; set; }
        public string Email { get; set; }
    }
}
