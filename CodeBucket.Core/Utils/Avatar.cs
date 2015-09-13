using System;
using System.Text.RegularExpressions;

namespace CodeBucket.Core.Utils
{
    public class Avatar
    {
        private readonly string _url;

        public Avatar(string url)
        {
            _url = url;
        }

        public string ToUrl(int size = 64)
        {
            if (_url == null)
                return null;

            var ret = Regex.Replace(_url, "/avatar/(\\d+)", "/avatar/" + size);
            return ret;
        }
    }
}

