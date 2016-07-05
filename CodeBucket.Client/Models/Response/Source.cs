using System.Collections.Generic;

namespace CodeBucket.Client.V1
{
    public class GitReference
    {
        public string Name { get; set; }
        public string Node { get; set; }
        public List<ChangesetFile> Files { get; set; }
        public string RawAuthor { get; set; }
        public string Author { get; set; }
        public string Timestamp { get; set; }
        public string RawNode { get; set; }
        public List<string> Parents { get; set; }
        public string Branch { get; set; }
        public string Message { get; set; }
        public string Revision { get; set; }
        public long Size { get; set; }
    }

    public class SourceDirectory
    {
        public List<string> Directories { get; set; }
        public List<FileModel> Files { get; set; } 

        public class FileModel
        {
            public string Path { get; set; }
            public string Revision { get; set; }
            public long Size { get; set; }
            public string Timestamp { get; set; }
        }
    }

    public class FileModel
    {
        public string Data { get; set; }
        public string Node { get; set; }
        public string Path { get; set; }
        public string Encoding { get; set; }
    }
}
