using System.Collections.Generic;

namespace CodeBucket.Client.V1
{
    public class Group
    {
        public string Name { get; set; }
        public string Permission { get; set; }
        public bool AutoAdd { get; set; }
        public List<User> Members { get; set; }
        public User Owner { get; set; }
        public string Slug { get; set; }
    }
}
