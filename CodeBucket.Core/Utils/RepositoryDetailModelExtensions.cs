using System;
using BitbucketSharp.Models;
using System.Text.RegularExpressions;

namespace BitbucketSharp.Models
{
    public static class RepositoryDetailModelExtensions
    {
        /// <summary>
        /// For some strange reason, bitbucket returns the smallest images possible to us via the repository detailed model
        /// That's completely absurd. So we'll try to make it bigger by manipulation of the URL based on some patterns I've noticed
        /// </summary>
        /// <returns>The logo.</returns>
        /// <param name="model">Model.</param>
        /// <param name="size">Size.</param>
        public static string LargeLogo(this RepositoryDetailedModel model, int size)
        {
            var logo = model.Logo;
            if (logo == null)
                return null;

            Console.WriteLine(logo);
            var match = Regex.Match(logo, @"(.*/img/language-avatars/.*)_\d+.png$", RegexOptions.IgnoreCase);
            if (match.Success)
                if (match.Groups.Count >= 2)
                    return match.Groups[1].Value + "_" + size + ".png";
            return logo;
        }
    }
}

