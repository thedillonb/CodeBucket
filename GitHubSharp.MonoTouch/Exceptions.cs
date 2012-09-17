using System;
using System.Net;

namespace GitHubSharp
{
    public class ForbiddenException : StatusCodeException
    {
        public ForbiddenException()
        : base(HttpStatusCode.Forbidden, "You do not have the permissions to access or modify this resource.") { }
    }

    public class NotFoundException : StatusCodeException
    {
        public NotFoundException()
        : base(HttpStatusCode.NotFound, "The server is unable to locate the requested resource.") { }
    }

    public class InternalServerException : StatusCodeException
    {
        public InternalServerException()
        : base(HttpStatusCode.InternalServerError, "The request was unable to be processed due to an interal server error.") { }
    }

    public class StatusCodeException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public StatusCodeException(HttpStatusCode statusCode)
            : this(statusCode, statusCode.ToString())
        {
        }

        public StatusCodeException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public static StatusCodeException FactoryCreate(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.Forbidden:
                    return new ForbiddenException();
                case HttpStatusCode.NotFound:
                    return new NotFoundException();
                case HttpStatusCode.InternalServerError:
                    return new InternalServerException();
                default:
                    return new StatusCodeException(statusCode);
            }
        }
    }
}

