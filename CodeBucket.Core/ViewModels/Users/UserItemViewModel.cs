using System.Reactive;
using CodeBucket.Core.Utils;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Users
{
    public class UserItemViewModel : ReactiveObject, ICanGoToViewModel
    {
        public string Username { get; }

        public string DisplayName { get; }

        public Avatar Avatar { get; }

        public ReactiveCommand<Unit, Unit> GoToCommand { get; } = ReactiveCommandFactory.Empty();

        public UserItemViewModel(string username, string displayName, Avatar avatar)
        {
            Username = username;
            Avatar = avatar;
            DisplayName = string.IsNullOrEmpty(displayName) ? string.Empty : displayName;
        }
    }
}

