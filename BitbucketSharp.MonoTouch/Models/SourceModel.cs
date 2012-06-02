using System;
using System.Collections.Generic;

namespace BitbucketSharp.Models
{
    public class SourceModel
    {
        public string Node { get; set; }
        public string Path { get; set; }
        public List<string> Directories { get; set; }
        public List<FileModel> Files { get; set; }

        public class FileModel
        {
            public string Path { get; set; }
            public string Revision { get; set; }
            public int Size { get; set; }
            public string Timestamp { get; set; }
        }
    }


}
