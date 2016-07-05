using System.Reactive;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels
{
    public interface IPaginatableViewModel
    {
        IReactiveCommand<Unit> LoadMoreCommand { get; }
    }
}

