using System;

namespace CodeBucket.Core.ViewModels
{
    public interface IRoutingViewModel
    {
        IObservable<IBaseViewModel> RequestNavigation { get; }
    }
}

