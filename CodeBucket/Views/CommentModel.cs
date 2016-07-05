using System.Collections.Generic;
using System.Linq;

namespace CodeBucket.Views
{
    public class Comment
    {
        public string AvatarUrl { get; set; }

        public string Name { get; set; }

        public string Date { get; set; }

        public string Body { get; set; }

        public Comment(string avatar, string name, string body, string date)
        {
            AvatarUrl = avatar;
            Name = name;
            Body = body;
            Date = date;
        }
    }

    public class CommentModel
    {
        public IList<Comment> Comments { get; set; }

        public int FontSize { get; set; }

        public CommentModel(IEnumerable<Comment> comments, int fontSize)
        {
            Comments = (comments ?? Enumerable.Empty<Comment>()).ToList();
            FontSize = fontSize;
        }
    }
}

