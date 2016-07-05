using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CodeBucket.Core.Services;

namespace CodeBucket.Core.Utils
{
    public class LoadingMessageHandler : DelegatingHandler
    {
        private readonly ILoadingIndicatorService _loadingService;

        public LoadingMessageHandler(ILoadingIndicatorService loadingService)
                : base(new HttpClientHandler())
        {
            _loadingService = loadingService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _loadingService.Up();

            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            finally
            {
                _loadingService.Down();
            }
        }
    }
}
