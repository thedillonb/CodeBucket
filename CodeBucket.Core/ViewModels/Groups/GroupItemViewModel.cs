using System.Reactive;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Groups
{
    public class GroupItemViewModel : ReactiveObject, ICanGoToViewModel
    {
        public string Name { get; }

        public ReactiveCommand<Unit, Unit> GoToCommand { get; } = ReactiveCommandFactory.Empty();

        public GroupItemViewModel(string name)
        {
            Name = name;
        }
    }
}

