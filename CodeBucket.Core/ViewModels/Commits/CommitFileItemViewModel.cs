using CodeBucket.Client;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Commits
{
    public class CommitFileItemViewModel : ReactiveObject
    {
        public IReactiveCommand<object> GoToCommand { get; } = ReactiveCommand.Create();

        public string Name { get; }

        public string Parent { get; }

        public string Username { get; }

        public string Repository { get; }

        public FileModification Type { get; }

        internal CommitFileItemViewModel(string fullPath, FileModification type)
        {
            var lastDirectoryMarker = fullPath.LastIndexOf('/');
            Name = fullPath.Substring(lastDirectoryMarker + 1);
            Type = type;

            var baseMarker = lastDirectoryMarker < 0 ? 0 : lastDirectoryMarker;
            Parent = "/" + fullPath.Substring(0, baseMarker);
        }
    }
}

