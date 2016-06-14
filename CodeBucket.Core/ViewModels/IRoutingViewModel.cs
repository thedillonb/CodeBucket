using System;

namespace CodeBucket.Core.ViewModels
{
    public interface IRoutingViewModel
    {
        IObservable<IViewModel> RequestNavigation { get; }
    }
}

