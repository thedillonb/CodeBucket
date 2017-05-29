using System.Reactive;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Source
{
    public class GitReferenceItemViewModel : ReactiveObject, ICanGoToViewModel
    {
        public string Name { get; }

        public ReactiveCommand<Unit, Unit> GoToCommand { get; } = ReactiveCommandFactory.Empty();

        public GitReferenceItemViewModel(string name)
        {
            Name = name;
        }
    }
}

