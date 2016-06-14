using ReactiveUI;
using System.Reactive.Subjects;
using System;

namespace CodeBucket.Core.ViewModels
{
    public abstract class BaseViewModel : ReactiveObject, IBaseViewModel
    {
        private readonly ISubject<IViewModel> _requestNavigationSubject = new Subject<IViewModel>();

        private string _title;
        public string Title
        {
            get { return _title; }
            protected set { this.RaiseAndSetIfChanged(ref _title, value); }
        }

        protected void NavigateTo(IViewModel viewModel)
        {
            _requestNavigationSubject.OnNext(viewModel);
        }

        IObservable<IViewModel> IRoutingViewModel.RequestNavigation
        {
            get { return _requestNavigationSubject; }
        }
    }
}
