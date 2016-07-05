namespace CodeBucket.Client.V1
{
    public class NewIssue
    {
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Title { get; set; }
        public string Responsible { get; set; }
        public string Content { get; set; }
        public string Kind { get; set; }
        public string Component { get; set; }
        public string Milestone { get; set; }
        public string Version { get; set; }

        public bool CheckNoChange()
        {
            return Status == null && Priority == null && Title == null && Responsible == null && Content == null &&                   Kind == null && Component == null && Milestone == null && Version == null;
        }
    }
}

