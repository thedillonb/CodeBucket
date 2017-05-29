using System.Reactive;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueAttributeItemViewModel : ReactiveObject
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { this.RaiseAndSetIfChanged(ref _isSelected, value); }
        }

        public string Name { get; }

        public ReactiveCommand<Unit, Unit> SelectCommand { get; } = ReactiveCommandFactory.Empty();

        public IssueAttributeItemViewModel(string name, bool selected = false)
        {
            Name = name;
            IsSelected = selected;
        }
    }
}

