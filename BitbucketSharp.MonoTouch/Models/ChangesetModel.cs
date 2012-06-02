using System.Collections.Generic;

namespace BitbucketSharp.Models
{
    public class ChangesetsModel
    {
        public int Count { get; set; }
        public string Start { get; set; }
        public int Limit { get; set; }
    }

    public class ChangesetModel
    {
        public string Node { get; set; }
        public string Author { get; set; }
        public string Timestamp { get; set; }
        public string Branch { get; set; }
        public string Message { get; set; }
        public int Revision { get; set; }
        public int Size { get; set; }
        public List<FileModel> Files { get; set; } 

        public class FileModel
        {
            public string Type { get; set; }
            public string File { get; set; }
        }
    }

    public class ChangesetDiffModel
    {
        public string Type { get; set; }
        public string File { get; set; }
        public List<DiffModel> Diffstat { get; set; } 

        public class DiffModel
        {
            public int Removed { get; set; }
            public int Added { get; set; }
        }
    }
}
