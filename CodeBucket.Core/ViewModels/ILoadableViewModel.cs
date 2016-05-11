using System.Reactive;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels
{
    public interface ILoadableViewModel
    {
        IReactiveCommand<Unit> LoadCommand { get; }
    }
}

