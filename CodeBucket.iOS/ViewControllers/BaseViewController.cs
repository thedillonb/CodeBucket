using System;
using ReactiveUI;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using Foundation;
using CodeBucket.Core.ViewModels;
using UIKit;
using CodeBucket.Views;
using System.Reactive.Disposables;
using Splat;
using CodeBucket.Services;

namespace CodeBucket.ViewControllers
{
    public abstract class BaseTableViewController<TViewModel> : BaseViewController<TViewModel> where TViewModel : class
    {
        private readonly Lazy<EnhancedTableView> _tableView;

        public EnhancedTableView TableView => _tableView.Value;

        protected BaseTableViewController(UITableViewStyle style = UITableViewStyle.Plain)
        {
             _tableView = new Lazy<EnhancedTableView>(() => new EnhancedTableView(style));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.AddTableView(TableView);

            OnActivation(disposable =>
            {
                var loadable =
                    this.WhenAnyValue(x => x.ViewModel)
                    .OfType<ILoadableViewModel>()
                    .Select(x => x.LoadCommand.IsExecuting)
                    .Switch()
                    .StartWith(false);

                var paginatable =
                    this.WhenAnyValue(x => x.ViewModel)
                    .OfType<IPaginatableViewModel>()
                    .Select(x => x.LoadMoreCommand.IsExecuting)
                    .Switch()
                    .StartWith(false);

                Observable.CombineLatest(loadable, paginatable, (l, p) => l || p)
                    .Subscribe(x => TableView.IsLoading = x)
                    .AddTo(disposable);
            });

        }

        protected override void Dispose(bool disposing)
        {
            if (_tableView.IsValueCreated)
            {
                var tableView = _tableView.Value;
                InvokeOnMainThread(() =>
                {
                    tableView.Source?.Dispose();
                    tableView.Source = null;
                });
            }

            base.Dispose(disposing);
        }
    }

    public abstract class BaseViewController<TViewModel> : BaseViewController, IViewFor<TViewModel> where TViewModel : class
    {
        private TViewModel _viewModel;
        public TViewModel ViewModel
        {
            get { return _viewModel; }
            set { this.RaiseAndSetIfChanged(ref _viewModel, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (TViewModel)value; }
        }

        protected BaseViewController()
        {
            Appearing
                .Take(1)
                .Select(_ => this.WhenAnyValue(x => x.ViewModel))
                .Switch()
                .OfType<ILoadableViewModel>()
                .Select(x => x.LoadCommand)
                .Subscribe(x => x.ExecuteIfCan());

            OnActivation(disposable =>
            {
                this.WhenAnyValue(x => x.ViewModel)
                    .OfType<IProvidesTitle>()
                    .Select(x => x.WhenAnyValue(y => y.Title))
                    .Switch()
                    .Subscribe(x => Title = x)
                    .AddTo(disposable);

                this.WhenAnyValue(x => x.ViewModel)
                    .OfType<IRoutingViewModel>()
                    .Select(x => x.RequestNavigation)
                    .Switch()
                    .Select(x => Locator.Current.GetService<IViewLocatorService>().GetView(x))
                    .OfType<UIViewController>()
                    .Subscribe(Navigate)
                    .AddTo(disposable);
            });
        }

        protected virtual void Navigate(UIViewController viewController)
        {
            NavigationController.PushViewController(viewController, true);
        }
    }

    public abstract class BaseViewController : ReactiveViewController, IActivatable
    {
        private readonly ISubject<bool> _appearingSubject = new Subject<bool>();
        private readonly ISubject<bool> _appearedSubject = new Subject<bool>();
        private readonly ISubject<bool> _disappearingSubject = new Subject<bool>();
        private readonly ISubject<bool> _disappearedSubject = new Subject<bool>();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        #if DEBUG
        ~BaseViewController()
        {
            Console.WriteLine("All done with " + GetType().Name);
        }
        #endif

        public IObservable<bool> Appearing => _appearingSubject.AsObservable();

        public IObservable<bool> Appeared => _appearedSubject.AsObservable();

        public IObservable<bool> Disappearing => _disappearingSubject.AsObservable();

        public IObservable<bool> Disappeared => _disappearedSubject.AsObservable();

        public void OnActivation(Action<CompositeDisposable> d)
        {
            Appearing.Subscribe(_ => d(_disposables));
        }

        protected BaseViewController()
        {
            CommonConstructor();
        }

        protected BaseViewController(string nib, NSBundle bundle)
            : base(nib, bundle)
        {
            CommonConstructor();
        }

        private void CommonConstructor()
        {
            this.WhenActivated(_ => { });
            NavigationItem.BackBarButtonItem = new UIBarButtonItem { Title = string.Empty };
        }

        private void DisposeActivations()
        {
            foreach (var disposable in _disposables)
                disposable.Dispose();
            _disposables.Clear();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            DisposeActivations();
            _appearingSubject.OnNext(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            _appearedSubject.OnNext(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            DisposeActivations();
            _disappearingSubject.OnNext(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            _disappearedSubject.OnNext(animated);
        }

        protected override void Dispose(bool disposing)
        {
            InvokeOnMainThread(() => View.DisposeAll());
            base.Dispose(disposing);
        }
    }
}

