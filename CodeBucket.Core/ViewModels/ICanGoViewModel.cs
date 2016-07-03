using ReactiveUI;

namespace CodeBucket.Core.ViewModels
{
    public interface ICanGoToViewModel
    {
        IReactiveCommand<object> GoToCommand { get; }
    }
}
