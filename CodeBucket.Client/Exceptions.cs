using System;
using System.Net;

namespace CodeBucket.Client
{
    public class BitbucketException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public BitbucketException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}

