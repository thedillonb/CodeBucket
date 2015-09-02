using System.Net.Http;
using CodeBucket.Core.Services;

namespace CodeBucket.Services
{
	public class HttpClientService : IHttpClientService
    {
		public HttpClient Create()
		{
			return new HttpClient(); //new ModernHttpClient.AFNetworkHandler()
		}
    }
}

