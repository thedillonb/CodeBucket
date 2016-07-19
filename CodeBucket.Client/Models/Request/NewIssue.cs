using System.Collections.Generic;

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
            return Status == null && Priority == null && Title == null && Responsible == null && Content == null &&
                   Kind == null && Component == null && Milestone == null && Version == null;
        }

        internal IEnumerable<KeyValuePair<string, string>> GetPairs()
        {
            yield return new KeyValuePair<string, string>("status", Status);
            yield return new KeyValuePair<string, string>("priority", Priority);
            yield return new KeyValuePair<string, string>("title", Title);
            yield return new KeyValuePair<string, string>("responsible", Responsible);
            yield return new KeyValuePair<string, string>("content", Content);
            yield return new KeyValuePair<string, string>("kind", Kind);
            yield return new KeyValuePair<string, string>("component", Component);
            yield return new KeyValuePair<string, string>("milestone", Milestone);
            yield return new KeyValuePair<string, string>("version", Version);
        }
    }
}

