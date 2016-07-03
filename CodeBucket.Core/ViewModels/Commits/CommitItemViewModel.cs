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

        public IReactiveCommand<object> GoToCommand { get; } = ReactiveCommand.Create();

        public CommitItemViewModel(string name, string description, string date, Avatar avatar)
        {
            Name = name;
            Description = description;
            Date = date;
            Avatar = avatar;
        }
    }
}
