using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Groups
{
    public class GroupItemViewModel : ReactiveObject, ICanGoToViewModel
    {
        public string Name { get; }

        public IReactiveCommand<object> GoToCommand { get; } = ReactiveCommand.Create();

        public GroupItemViewModel(string name)
        {
            Name = name;
        }
    }
}
