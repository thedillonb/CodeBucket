using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CodeBucket.Core.Stores;
using CodeBucket.DialogElements;

namespace CodeBucket.ViewControllers
{
    public abstract class SelectionViewController<T> : DialogViewController
    {
        private readonly LoadableResource<ImmutableList<T>> _resources;
        private readonly ISubject<T> _selectedSubject = new Subject<T>();

        public IObservable<T> SelectedValueChanged 
            => _selectedSubject.AsObservable();

        public ImmutableList<T> Items => _resources.Resource;

        private T _selectedValue;
        public T SelectedValue
        {
            get { return _selectedValue; }
            set 
            {
                if (Equals(value, _selectedValue))
                    return;

                _selectedValue = value;
                Render();
            }
        }

        protected SelectionViewController(LoadableResource<ImmutableList<T>> resources)
            : base(UIKit.UITableViewStyle.Plain)
        {
            _resources = resources;

            EnableSearch = false;

            OnActivation(disposable =>
            {
                _resources
                    .IsLoadingChanged
                    .Subscribe(x => TableView.IsLoading = x)
                    .AddTo(disposable);
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //_resources.Retrieve().Subscribe(_ => Render());
        }

        private void Render()
        {
            Root.Reset(new Section
            {
                Items.Select(x => {
                    var element = CreateElement(x);
                    element.Checked = Equals(SelectedValue, x);
                    return element;
                })
            });
        }

        protected abstract CheckElement CreateElement(T item);
    }
}

