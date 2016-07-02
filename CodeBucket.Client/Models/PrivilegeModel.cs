using System.Collections.Generic;

namespace CodeBucket.Client.Models
{
    public class PrivilegeModel
    {
        public string Repo { get; set; }
        public string Privilege { get; set; }
        public UserModel User { get; set; }
    }

    public class GroupPrivilegeModel
    {
        public string Repo { get; set; }
        public string Privilege { get; set; }
        public GroupModel Group { get; set; }
        public RepoModel Repository { get; set; }

        public class RepoModel
        {
            public UserModel Owner { get; set; }
            public string Name { get; set; }
            public string Slug { get; set; }
        }
    }

    public class AccountPrivileges
    {
        public Dictionary<string, string> Teams { get; set; }
    }
}

