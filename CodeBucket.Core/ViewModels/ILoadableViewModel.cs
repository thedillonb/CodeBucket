using System.Reactive;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels
{
    public interface ILoadableViewModel
    {
        ReactiveCommand<Unit, Unit> LoadCommand { get; }
    }
}

