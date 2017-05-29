using System.Reactive;
using CodeBucket.Core.Utils;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueAssigneeItemViewModel : ReactiveObject
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { this.RaiseAndSetIfChanged(ref _isSelected, value); }
        }

        public string Name { get; }

        public Avatar Avatar { get; }

        public ReactiveCommand<Unit, Unit> SelectCommand { get; } = ReactiveCommandFactory.Empty();

        public IssueAssigneeItemViewModel(string name, string avatar, bool selected = false)
        {
            Name = name;
            IsSelected = selected;
            Avatar = new Avatar(avatar);
        }
    }
}

