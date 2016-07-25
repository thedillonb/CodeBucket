using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeBucket.Views
{
    public class DiffViewModel
    {
        public List<Hunk> Patch { get; }

        public List<CommitComment> Comments { get; }

        public DiffViewModel(IEnumerable<Hunk> patch, IEnumerable<CommitComment> comments)
        {
            Patch = patch.ToList();
            Comments = (comments ?? Enumerable.Empty<CommitComment>()).ToList();
        }
    }

    public class CommitComment
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public int? LineTo { get; set; }
        public int? LineFrom { get; set; }
        public string Content { get; set; }
        public DateTimeOffset Date { get; set; }
    }

    public class Hunk
    {
        public int OldStart { get; }
        public int NewStart { get; }
        public IList<string> Lines { get; }

        public Hunk(int oldStart, int newStart, IList<string> lines)
        {
            OldStart = oldStart;
            NewStart = newStart;
            Lines = lines;
        }
    }
}

