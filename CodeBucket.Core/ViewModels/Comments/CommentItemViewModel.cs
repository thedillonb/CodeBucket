using CodeBucket.Core.Utils;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Comments
{
    public class CommentItemViewModel : ReactiveObject
    {
        public string Name { get; }

        public Avatar Avatar { get; }

        public string Content { get; }

        public string CreatedOn { get; }

        public CommentItemViewModel(string name, Avatar avatar, string createdOn, string content)
        {
            Name = name;
            Avatar = avatar;
            CreatedOn = createdOn;
            Content = content;
        }
    }
}
