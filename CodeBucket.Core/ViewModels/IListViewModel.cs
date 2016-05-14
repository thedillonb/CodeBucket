using ReactiveUI;

namespace CodeBucket.Core.ViewModels
{
    public interface IListViewModel<T>
    {
        IReadOnlyReactiveList<T> Items { get; }
    }
}

