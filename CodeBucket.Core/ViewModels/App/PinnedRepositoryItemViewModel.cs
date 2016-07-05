using CodeBucket.Core.Utils;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.App
{
    public class PinnedRepositoryItemViewModel : ReactiveObject
    {
        public string Name { get; }

        public Avatar Avatar { get; }

        public IReactiveCommand<object> DeleteCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> GoToCommand { get; } = ReactiveCommand.Create();

        public PinnedRepositoryItemViewModel(string name, Avatar avatar)
        {
            Name = name;
            Avatar = avatar;
        }
    }
}

