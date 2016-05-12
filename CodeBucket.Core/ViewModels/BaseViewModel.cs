using ReactiveUI;
using System.Reactive.Subjects;
using System;

namespace CodeBucket.Core.ViewModels
{
    public abstract class BaseViewModel : ReactiveObject, IBaseViewModel
    {
        private readonly ISubject<IBaseViewModel> _requestNavigationSubject = new Subject<IBaseViewModel>();

        private string _title;
        public string Title
        {
            get { return _title; }
            protected set { this.RaiseAndSetIfChanged(ref _title, value); }
        }

        protected void NavigateTo(IBaseViewModel viewModel)
        {
            _requestNavigationSubject.OnNext(viewModel);
        }

        IObservable<IBaseViewModel> IRoutingViewModel.RequestNavigation
        {
            get { return _requestNavigationSubject; }
        }
    }
}
