using System.Collections.Generic;

namespace CodeBucket.Core.Services
{
    public interface IDiffService
    {
        IEnumerable<Hunk> CreateDiff(string oldContent, string newContent, int context);
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

