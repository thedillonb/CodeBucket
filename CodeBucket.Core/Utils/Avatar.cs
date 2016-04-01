using System;
using System.Text.RegularExpressions;

namespace CodeBucket.Core.Utils
{
    public class Avatar
    {
        public string Url { get; }

        public Avatar(string url)
        {
            Url = url;
        }
    }

    public static class AvatarExtensions
    {
        public static string ToUrl(this Avatar @this, int? size = null)
        {
            if (@this.Url == null)
                return null;

            if (!size.HasValue)
                return @this.Url;

            return Regex.Replace(@this.Url, "/avatar/(\\d+)", "/avatar/" + size.Value);
        }

        public static Uri ToUri(this Avatar @this, int? size = null)
        {
            var url = @this.ToUrl(size);
            if (url == null)
                return null;

            Uri uri;
            Uri.TryCreate(url, UriKind.Absolute, out uri);
            return uri;
        }
    }
}

