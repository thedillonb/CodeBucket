using CodeBucket.Core.Utils;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoryItemViewModel : ReactiveObject
    {
        public IReactiveCommand<object> GoToCommand { get; } = ReactiveCommand.Create();

        public string Name { get; }

        public string Description { get; }

        public Avatar Avatar { get; }

        public string Owner { get; }

        public RepositoryItemViewModel(string name, string description, string owner, Avatar avatar)
        {
            Name = name;
            Description = description;
            Owner = owner;
            Avatar = avatar;
        }
    }
}

