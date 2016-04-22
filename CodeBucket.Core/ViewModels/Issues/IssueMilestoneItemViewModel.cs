using System;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueMilestoneItemViewModel : ReactiveObject
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { this.RaiseAndSetIfChanged(ref _isSelected, value); }
        }

        public string Name { get; }

        public IReactiveCommand<object> SelectCommand { get; } = ReactiveCommand.Create();

        public IssueMilestoneItemViewModel(string name, bool selected = false)
        {
            Name = name;
            IsSelected = selected;
            SelectCommand.Subscribe(x => IsSelected = !IsSelected);
        }
    }
}

