using System.Reactive;
using CodeBucket.Core.Utils;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Commits
{
    public class CommitItemViewModel : ReactiveObject, ICanGoToViewModel
    {
        public Avatar Avatar { get; }

        public string Name { get; }

        public string Description { get; }

        public string Date { get; }

        public string Sha { get; }

        public ReactiveCommand<Unit, Unit> GoToCommand { get; } = ReactiveCommandFactory.Empty();

        public CommitItemViewModel(string name, string description, string date, Avatar avatar, string sha)
        {
            Name = name;
            Description = description;
            Date = date;
            Avatar = avatar;
            Sha = sha;
        }
    }
}

