using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Source
{
    public class GitReferenceItemViewModel : ReactiveObject, ICanGoToViewModel
    {
        public string Name { get; }

        public IReactiveCommand<object> GoToCommand { get; } = ReactiveCommand.Create();

        public GitReferenceItemViewModel(string name)
        {
            Name = name;
        }
    }
}

