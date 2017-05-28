using System;
namespace CodeBucket.Core
{
    public static class Secrets
    {
        public static string ClientId
        {
            get { throw new Exception("Must supply your own ClientId"); }
        }

        public static string ClientSecret
        {
            get { throw new Exception("Must supply your own ClientSecret"); }
        }
    }
}
