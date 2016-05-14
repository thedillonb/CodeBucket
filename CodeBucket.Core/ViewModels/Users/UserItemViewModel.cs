using CodeBucket.Core.Utils;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Users
{
    public class UserItemViewModel : ReactiveObject
    {
        public string Username { get; }

        public string DisplayName { get; }

        public Avatar Avatar { get; }

        public IReactiveCommand<object> GoToCommand { get; } = ReactiveCommand.Create();

        public UserItemViewModel(string username, string displayName, Avatar avatar)
        {
            Username = username;
            Avatar = avatar;
            DisplayName = string.IsNullOrEmpty(displayName) ? string.Empty : displayName;
        }
    }
}

