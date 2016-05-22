using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CodeBucket.Core.Services;
using ReactiveUI;
using UIKit;

namespace CodeBucket.Services
{
    public class LoadingIndicatorService : ILoadingIndicatorService
    {
        private readonly ISubject<int> _activeSubject = new Subject<int>();
        private static UIApplication _app = UIApplication.SharedApplication;
        static readonly object NetworkLock = new object();
        static int _active;

        public LoadingIndicatorService()
        {
            _activeSubject
                .Throttle(TimeSpan.FromMilliseconds(100))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => _app.NetworkActivityIndicatorVisible = x > 0);
        }

        public void Down()
        {
            lock (NetworkLock)
            {
                if (_active == 0)
                    return;

                _active--;
                _activeSubject.OnNext(_active);
            }
        }

        public void Up()
        {
            lock (NetworkLock)
            {
                _active++;
                _activeSubject.OnNext(_active);
            }
        }
    }
}

