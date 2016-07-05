using System.Collections.Generic;
using System;

namespace CodeBucket.Client.V1
{
    public class Repository
    {
        public string Description { get; set; }
        public int ForkCount { get; set; }
        public string ResourceUri { get; set; }
        public string Website { get; set; }
        public bool HasWiki { get; set; }
        public string LastUpdated { get; set; }
        public DateTimeOffset UtcLastUpdated { get; set; }
        public string CreatedOn { get; set; }
        public DateTimeOffset UtcCreatedOn { get; set; }
        public string Logo { get; set; }
        public long Size { get; set; }
        public bool ReadOnly { get; set; }
        public string Language { get; set; }
        public int FollowersCount { get; set; }
        public string State { get; set; }
        public bool HasIssues { get; set; }
        public bool IsFork { get; set; }
        public bool EmailWriters { get; set; }
        public bool NoPublicForks { get; set; }
        public Repository ForkOf { get; set; }
        public bool IsPrivate { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public string Scm { get; set; }
    }
}

namespace CodeBucket.Client
{
    //Cant use any of the above :(
    public class RepositorySearch
    {
        public int Count { get; set; }
        public string Query { get; set; }
        public List<Repository> Repositories { get; set; }
    }

    public class RepositoryFollowModel
    {
        public int Followers { get; set; }
        public bool Following { get; set; }
        public string Type { get; set; }
    }

    public class PrimaryBranchModel
    {
        public string Name { get; set; }
    }

    public class Repository
    {
        public DateTimeOffset CreatedOn { get; set; }
        public string Description { get; set; }
        public string ForkPolicy { get; set; }
        public string FullName { get; set; }
        public bool HasIssues { get; set; }
        public bool HasWiki { get; set; }
        public string Language { get; set; }
        public User Owner { get; set; }
        public string Name { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int Size { get; set; }
        public bool IsPrivate { get; set; }
        public string Scm { get; set; }
        public RepositoryLinks Links { get; set; }
        public Repository Parent { get; set; }
        public string Website { get; set; }

        public class RepositoryLinks
        {
            public Link Avatar { get; set; }
        }

    }
}
