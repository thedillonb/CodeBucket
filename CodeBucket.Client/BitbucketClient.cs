using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CodeBucket.Client
{
    public class BitbucketClient
    {
        private readonly AuthenticationHeaderValue _authorizationHeader;
        public const string ApiUrl = "https://api.bitbucket.org/1.0";
        public const string ApiUrl2 = "https://api.bitbucket.org/2.0";
        public static string Url = "https://bitbucket.org";

        private static readonly Lazy<JsonSerializer> _serializer = new Lazy<JsonSerializer>(() =>
        {
            var serializer = new JsonSerializer();
            serializer.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            serializer.NullValueHandling = NullValueHandling.Ignore;
            return serializer;
        });

        /// <summary>
        /// The username we are logging as as.
        /// Instead of passing in the account's username everywhere we'll just set it once here.
        /// </summary>
        public string Username { get; set; }

        public TeamsClient Teams => new TeamsClient(this);

        public CommitsClient Commits => new CommitsClient(this);

        public GroupsClient Groups => new GroupsClient(this);

        public IssuesClient Issues => new IssuesClient(this);

        public PrivilegesClient Privileges => new PrivilegesClient(this);

        public PullRequestsClient PullRequests => new PullRequestsClient(this);

        public RepositoriesClient Repositories => new RepositoriesClient(this);

        public UsersClient Users => new UsersClient(this);

        private BitbucketClient(AuthenticationHeaderValue authorizationHeader)
        {
            _authorizationHeader = authorizationHeader;
        }

        public static BitbucketClient WithBasicAuthentication(string username, string password)
        {
            var byteArray = Encoding.UTF8.GetBytes(username + ":" + password);
            return new BitbucketClient(new AuthenticationHeaderValue("basic", Convert.ToBase64String(byteArray)));
        }

        /// <summary>
        /// Create a Client object and request the user's info to validate credentials
        /// </summary>
        public static BitbucketClient WithBearerAuthentication(string token)
        {
            return new BitbucketClient(new AuthenticationHeaderValue("Bearer", token));
        }

        public static async Task<OAuthResponse> GetRefreshToken(string clientId, string clientSecret, string refreshToken)
        {
            var client = WithBasicAuthentication(clientId, clientSecret);
            var uri = new Uri("https://bitbucket.org/site/oauth2/access_token");
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            var data = new Dictionary<string, string>();
            data.Add("grant_type", "refresh_token");
            data.Add("refresh_token", refreshToken);
            request.Content = new FormUrlEncodedContent(data);
            request.Headers.Add("Accept", "application/json");
            var resp = await client.ExecuteRequest(request).ConfigureAwait(false);
            return await ParseBody<OAuthResponse>(resp).ConfigureAwait(false);
        }

        public static async Task<OAuthResponse> GetAuthorizationCode(string clientId, string clientSecret, string code)
        {
            var client = WithBasicAuthentication(clientId, clientSecret);
            var uri = new Uri("https://bitbucket.org/site/oauth2/access_token");
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            var data = new Dictionary<string, string>();
            data.Add("grant_type", "authorization_code");
            data.Add("code", code);
            request.Content = new FormUrlEncodedContent(data);
            request.Headers.Add("Accept", "application/json");
            var resp = await client.ExecuteRequest(request).ConfigureAwait(false);
            return await ParseBody<OAuthResponse>(resp).ConfigureAwait(false);
        }

        private static async Task<T> ParseBody<T>(HttpResponseMessage message)
        {
            var body = await message.Content.ReadAsStreamAsync().ConfigureAwait(false);
            using (var reader = new StreamReader(body))
            using (var textReader = new JsonTextReader(reader))
                return _serializer.Value.Deserialize<T>(textReader);
        }

        public async Task<T> Get<T>(string uri) where T : class
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("Accept", "application/json");
            var resp = await ExecuteRequest(request).ConfigureAwait(false);
            return await ParseBody<T>(resp).ConfigureAwait(false);
        }

        public Task<HttpResponseMessage> GetRaw(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return ExecuteRequest(request);
        }

        public async Task<T> Put<T>(string uri, object data)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, uri);
            var json = SerializeToJson(data);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Headers.Add("Accept", "application/json");
            if (json.Length == 0) request.Headers.Add("Content-Length", "0");
            var resp = await ExecuteRequest(request).ConfigureAwait(false);
            return await ParseBody<T>(resp).ConfigureAwait(false);
        }

        public Task Put(string uri, object data)
        {
            return Put<string>(uri, data);
        }

        public async Task<T> PostForm<T>(string uri, IEnumerable<KeyValuePair<string, string>> data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new FormUrlEncodedContent(data);
            request.Headers.Add("Accept", "application/json");
            var resp = await ExecuteRequest(request).ConfigureAwait(false);
            return await ParseBody<T>(resp).ConfigureAwait(false);
        }

        public async Task<T> Post<T>(string uri, object data = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            var json = SerializeToJson(data);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Headers.Add("Accept", "application/json");
            var resp = await ExecuteRequest(request).ConfigureAwait(false);
            return await ParseBody<T>(resp).ConfigureAwait(false);
        }

        private string SerializeToJson(object data)
        {
            var sb = new StringBuilder();
            using (var text = new StringWriter(sb))
                _serializer.Value.Serialize(text, data);
            return sb.ToString();
        }

        public Task Delete(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);
            return ExecuteRequest(request);
        }

        private async Task<HttpResponseMessage> ExecuteRequest(HttpRequestMessage request)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = _authorizationHeader;
            client.Timeout = TimeSpan.FromSeconds(30);
            var resp = await client.SendAsync(request).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                ErrorResponse error = null;

                if (body.StartsWith("{", StringComparison.Ordinal)
                    || body.StartsWith("[", StringComparison.Ordinal))
                {
                    try
                    {
                        error = JsonConvert.DeserializeObject<ErrorResponse>(body);
                    }
                    catch
                    {
                        /* Do nothing */
                    }
                }
                else
                {
                    error = new ErrorResponse
                    {
                        Error = new ErrorDetails
                        {
                            Message = body
                        }
                    };
                }

                if (string.IsNullOrEmpty(error?.Error?.Message))
                    throw new BitbucketException(resp.StatusCode, "Server returned an invalid status code: " + resp.StatusCode);
                throw new BitbucketException(resp.StatusCode, error.Error.Message);
            }

            return resp;
        }
    }

    public class OAuthResponse
    {
        public string AccessToken { get; set; }
        public string Scopes { get; set; }
        public string RefreshToken { get; set; }
    }

    public class ErrorResponse
    {
        public ErrorDetails Error { get; set ;}
    }

    public class ErrorDetails
    {
        public string Message { get; set; }
        public string Detail { get; set; }
    }
}
