using System.Reactive;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels
{
    public interface IPaginatableViewModel
    {
        ReactiveCommand<Unit, Unit> LoadMoreCommand { get; }
    }
}

