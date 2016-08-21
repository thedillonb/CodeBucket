using System;

namespace CodeBucket.Core.Data
{
    public class PinnedRepository
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Owner { get; set; }

        public string Slug { get; set; }

        public string Name { get; set; }

        public string ImageUri { get; set; }
    }
}

