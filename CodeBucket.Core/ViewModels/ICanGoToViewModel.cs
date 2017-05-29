using System.Reactive;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels
{
    public interface ICanGoToViewModel
    {
        ReactiveCommand<Unit, Unit> GoToCommand { get; }
    }
}

