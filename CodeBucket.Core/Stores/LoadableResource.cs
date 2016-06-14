using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace CodeBucket.Core.Stores
{
    public class LoadableResource<T>
    {
        private readonly ISubject<bool> _isLoadingSubject = new BehaviorSubject<bool>(false);
        private readonly Func<Task<T>> _loadFunc;

        public T Resource { get; private set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            private set
            {
                if (value == _isLoading)
                    return;
                _isLoading = value;
                _isLoadingSubject.OnNext(value);
            }
        }

        public IObservable<bool> IsLoadingChanged => _isLoadingSubject.AsObservable();

        //public IObservable<T> Retrieve(bool forceReload = false)
        //{
        //    IsLoading = true;

        //}

        public LoadableResource(Func<Task<T>> loadFunc, T defaultResource = default(T))
        {
            _loadFunc = loadFunc;
            Resource = defaultResource;
        }
    }
}

