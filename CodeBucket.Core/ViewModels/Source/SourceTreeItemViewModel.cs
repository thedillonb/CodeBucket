using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Source
{
    public class SourceTreeItemViewModel : ICanGoToViewModel
    {
        public string Name { get; }

        public SourceTreeItemType Type { get; }

        public IReactiveCommand<object> GoToCommand { get; } = ReactiveCommand.Create();

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
