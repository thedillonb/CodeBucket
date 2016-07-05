namespace CodeBucket.Client
{
    public class NewChangesetComment
    {
        public string Content { get; set; }
        public long? LineFrom { get; set; }
        public long? LineTo { get; set; }        public long? ParentId { get; set; }
        public string Filename { get; set; }
    }
}

