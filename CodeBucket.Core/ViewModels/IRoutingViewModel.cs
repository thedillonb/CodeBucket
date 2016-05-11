using System;
using System.Reactive;

namespace CodeBucket.Core.ViewModels
{
    public interface IRoutingViewModel
    {
        IObservable<IBaseViewModel> RequestNavigation { get; }

        IObservable<Unit> RequestDismiss { get; }
    }
}

