using System;

namespace CodeBucket.Core.Services
{
    public interface IErrorService
    {
        void Init(string sentryUrl, string sentryClientId, string sentrySecret);

        void ReportError(Exception e);
    }
}

