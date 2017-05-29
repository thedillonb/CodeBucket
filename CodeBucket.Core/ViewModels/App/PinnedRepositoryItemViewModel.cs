using System.Reactive;
using CodeBucket.Core.Utils;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.App
{
    public class PinnedRepositoryItemViewModel : ReactiveObject
    {
        public string Name { get; }

        public Avatar Avatar { get; }

        public ReactiveCommand<Unit, Unit> DeleteCommand { get; } = ReactiveCommandFactory.Empty();

        public ReactiveCommand<Unit, Unit> GoToCommand { get; } = ReactiveCommandFactory.Empty();

        public PinnedRepositoryItemViewModel(string name, Avatar avatar)
        {
            Name = name;
            Avatar = avatar;
        }
    }
}

