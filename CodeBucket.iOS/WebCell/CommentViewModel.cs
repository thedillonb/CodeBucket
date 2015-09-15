using System;
using Humanizer;

namespace CodeBucket.WebCell
{
    public class CommentViewModel
    {
        public CommentViewModel(string title, string content, DateTimeOffset created, string avatarUrl)
        {
            Title = title;
            Content = content;
            Created = created.Humanize();
            AvatarUrl = avatarUrl;
        }

        public string AvatarUrl { get; private set; }
        public string Title { get; private set; }
        public string Created { get; private set; }
        public string Content { get; private set; }
    }
}

