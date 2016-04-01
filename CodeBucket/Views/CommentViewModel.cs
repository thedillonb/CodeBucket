using System;

namespace CodeBucket.Views
{
    public class CommentViewModel
    {
        public CommentViewModel(string title, string content, string created, string avatarUrl)
        {
            Title = title;
            Content = content;
            Created = created;
            AvatarUrl = avatarUrl;
        }

        public string AvatarUrl { get; }
        public string Title { get; }
        public string Created { get; }
        public string Content { get; }
    }
}

