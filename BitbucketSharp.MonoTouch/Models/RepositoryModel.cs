namespace BitbucketSharp.Models
{
    public class RepositoryDetailedModel : RepositorySimpleModel
    {
        public string Description { get; set; }
        public int ForkCount { get; set; }
        public string ResourceUri { get; set; }
        public string Website { get; set; }
        public bool HasWiki { get; set; }
        public string LastUpdated { get; set; }
        public string CreatedOn { get; set; }
        public string Logo { get; set; }
        public int Size { get; set; }
        public bool ReadOnly { get; set; }
        public string Language { get; set; }
        public int FollowersCount { get; set; }
        public string State { get; set; }
        public bool HasIssues { get; set; }
        public bool IsFork { get; set; }
        public bool EmailWriters { get; set; }
        public bool NoPublicForks { get; set; }
    }

    public class RepositorySimpleModel
    {
        public bool IsPrivate { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public string Scm { get; set; }
    }
}
