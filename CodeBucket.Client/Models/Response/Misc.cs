using System.Collections.Generic;

namespace CodeBucket.Client
{
    public class Collection<T>
    {
        public int Size { get; set; }
        public int Page { get; set; }
        public uint Pagelen { get; set; }
        public string Next { get; set; }
        public string Previous { get; set; }
        public List<T> Values { get; set; }
    }

    public class Link
    {
        public string Href { get; set; }

        public Link() { }

        public Link(string href)
        {
            Href = href;
        }
    }
}
