using System.Reactive;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Source
{
    public class SourceTreeItemViewModel : ReactiveObject, ICanGoToViewModel
    {
        public string Name { get; }

        public SourceTreeItemType Type { get; }

        public ReactiveCommand<Unit, Unit> GoToCommand { get; } = ReactiveCommandFactory.Empty();

        public SourceTreeItemViewModel(string name, SourceTreeItemType type)
        {
            Name = name;
            Type = type;
        }

        public enum SourceTreeItemType
        {
            File,
            Directory,
            Submodule
        }
    }
}

