using System;
using System.Collections.Generic;

namespace BitbucketSharp.Models
{
    public class EmailModel
    {
        public bool Active { get; set; }
        public string Email { get; set; }
        public bool Primary { get; set; }
    }
}
