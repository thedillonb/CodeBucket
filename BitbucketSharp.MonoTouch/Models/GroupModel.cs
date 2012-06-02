using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitbucketSharp.Models
{
    public class GroupModel
    {
        public string Name { get; set; }
        public string Permission { get; set; }
        public bool AutoAdd { get; set; }
        public List<UserModel> Members { get; set; }
        public UserModel Owner { get; set; }
        public string Slug { get; set; }
    }
}
