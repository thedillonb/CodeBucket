using System.Collections.Generic;
using System.Linq;

namespace CodeBucket.Views
{
    public class DiffViewModel
    {
        public List<Hunk> Patch { get; }

        public DiffViewModel(IEnumerable<Hunk> patch)
        {
            Patch = patch.ToList();
        }
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

