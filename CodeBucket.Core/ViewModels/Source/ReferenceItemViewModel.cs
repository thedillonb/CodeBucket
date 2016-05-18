using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Source
{
    public class ReferenceItemViewModel : ReactiveObject, ICanGoToViewModel
    {
        public string Name { get; }

        public IReactiveCommand<object> GoToCommand { get; } = ReactiveCommand.Create();

        public ReferenceItemViewModel(string name)
        {
            Name = name;
        }
    }
}

