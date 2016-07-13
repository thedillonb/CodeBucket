using CodeBucket.Client;
using CodeBucket.Client.V1;
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

        public string Node { get; }

        public string ParentNode { get; }

        public ChangesetFile ChangesetFile { get; }

        public FileModification Type { get; }

        internal CommitFileItemViewModel(string username, string repository, string parentNode, string parentCommit, ChangesetFile file)
        {
            Username = username;
            Repository = repository;
            Node = parentNode;
            ChangesetFile = file;
            ParentNode = parentCommit;

            var fullPath = file.File;
            var lastDirectoryMarker = fullPath.LastIndexOf('/');
            Name = fullPath.Substring(lastDirectoryMarker + 1);
            Type = file.Type;

            var baseMarker = lastDirectoryMarker < 0 ? 0 : lastDirectoryMarker;
            Parent = "/" + fullPath.Substring(0, baseMarker);
        }
    }
}

