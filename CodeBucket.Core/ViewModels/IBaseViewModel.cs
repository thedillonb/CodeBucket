using ReactiveUI;

namespace CodeBucket.Core.ViewModels
{
    public interface IBaseViewModel : ISupportsActivation, IProvidesTitle, IRoutingViewModel
    {
    }
}

