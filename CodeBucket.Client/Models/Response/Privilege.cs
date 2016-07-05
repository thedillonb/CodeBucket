using System.Collections.Generic;

namespace CodeBucket.Client.V1
{
    public class PrivilegeModel
    {
        public string Repo { get; set; }
        public string Privilege { get; set; }
        public User User { get; set; }
    }

    public class GroupPrivilege
    {
        public string Repo { get; set; }
        public string Privilege { get; set; }
        public Group Group { get; set; }
        public RepositoryPrivilege Repository { get; set; }

        public class RepositoryPrivilege
        {
            public User Owner { get; set; }
            public string Name { get; set; }
            public string Slug { get; set; }
        }
    }

    public class AccountPrivileges
    {
        public Dictionary<string, string> Teams { get; set; }
    }
}

