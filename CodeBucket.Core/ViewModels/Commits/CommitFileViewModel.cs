using BitbucketSharp.Models;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Commits
{
    public class CommitFileViewModel
    {
        public IReactiveCommand<object> GoToCommand { get; } = ReactiveCommand.Create();

        public string Name { get; }

        public string Parent { get; }

        public ChangesetModel.FileType Type { get; }

        internal CommitFileViewModel(string fullPath, ChangesetModel.FileType type)
        {
            var lastDirectoryMarker = fullPath.LastIndexOf('/');
            Name = fullPath.Substring(lastDirectoryMarker + 1);
            Type = type;

            var baseMarker = lastDirectoryMarker < 0 ? 0 : lastDirectoryMarker;
            Parent = "/" + fullPath.Substring(0, baseMarker);
        }
    }
}

