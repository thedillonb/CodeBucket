using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GitHubSharp.Models
{
    public class Commit
    {
        public List<CommmitParent> Parents { get; set; }
        public Person Author { get; set; }
        public string Url { get; set; }
        public string Sha { get; set; }
        public string Message { get; set; }
        public CommmitParent Tree { get; set; }
        public Person Committer { get; set; }
    }

    public class CommmitParent
    {
        public string Sha { get; set; }
        public string Url { get; set; }
    }

    public class Person
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Email { get; set; }
    }

    public class SingleFileCommit : Commit
    {
        public List<SingleFileCommitFileReference> Added { get; set; }
        public List<SingleFileCommitFileReference> Removed { get; set; }
        public List<SingleFileCommitFileReference> Modified { get; set; }
    }

    public class SingleFileCommitFileReference
    {
        public string Filename { get; set; }
    }
}
