using System;
using System.Collections.Generic;
using System.Net;
using BitbucketSharp.Controllers;
using RestSharp;
using RestSharp.Deserializers;

namespace BitbucketSharp
{
    public class Client
    {
        private readonly RestClient _client = new RestClient("https://api.bitbucket.org/1.0");

        /// <summary>
        /// Gets the username for this clietn
        /// </summary>
        public String Username { get; private set; }

        /// <summary>
        /// The user account
        /// </summary>
        public AccountController Account { get; private set; }

        /// <summary>
        /// The users on Bitbucket
        /// </summary>
        public UsersController Users { get; private set; }

        /// <summary>
        /// The repositories on Bitbucket
        /// </summary>
        public RepositoriesController Repositories { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public Client(String username, String password)
        {
            Username = username;
            Account = new AccountController(this);
            Users = new UsersController(this);
            Repositories = new RepositoriesController(this);
            _client.Authenticator = new HttpBasicAuthenticator(username, password);
        }

        /// <summary>
        /// Makes a 'GET' request to the server using a URI
        /// </summary>
        /// <typeparam name="T">The type of object the response should be deserialized ot</typeparam>
        /// <param name="uri">The URI to request information from</param>
        /// <returns>An object with response data</returns>
        public T Get<T>(String uri)
        {
            return Request<T>(uri);
        }

        /// <summary>
        /// Makes a 'PUT' request to the server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        public T Put<T>(string uri, Dictionary<string, string> data = null)
        {
            return Request<T>(uri, Method.PUT, data);
        }

        /// <summary>
        /// Makes a 'PUT' request to the server
        /// </summary>
        /// <param name="uri"></param>
        public void Put(string uri, Dictionary<string, string> data = null)
        {
            Request(uri, Method.PUT, data);
        }

        /// <summary>
        /// Makes a 'POST' request to the server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public T Post<T>(string uri, Dictionary<string, string> data)
        {
            return Request<T>(uri, Method.POST, data);
        }

        /// <summary>
        /// Makes a 'POST' request to the server without a response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public void Post(string uri, Dictionary<string, string> data)
        {
            Request(uri, Method.POST, data);
        }

        /// <summary>
        /// Makes a 'DELETE' request to the server
        /// </summary>
        /// <param name="uri"></param>
        public void Delete(string uri)
        {
            Request<string>(uri, Method.DELETE);
        }

        /// <summary>
        /// Makes a request to the server expecting a response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="data"></param>
        /// <param name="header"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public T Request<T>(string uri, Method method = Method.GET, Dictionary<string, string> data = null)
        {
            var response = ExecuteRequest(uri, method, data);
            var d = new JsonDeserializer();
            return d.Deserialize<T>(response);
        }

        /// <summary>
        /// Makes a request to the server but does not expect a response.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <param name="header"></param>
        public void Request(string uri, Method method = Method.GET, Dictionary<string, string> data = null)
        {
            ExecuteRequest(uri, method, data);
        }

        /// <summary>
        /// Executes a request to the server
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private IRestResponse ExecuteRequest(string uri, Method method, Dictionary<string, string> data)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            var request = new RestRequest(uri, method);
            if (data != null)
                foreach (var hd in data)
                    request.AddParameter(hd.Key, hd.Value);
            
            //Puts without any data must be marked as having no content!
            if (method == Method.PUT && data == null)
                request.AddHeader("Content-Length", "0");

            var response = _client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException("Request returned status code: " + response.StatusCode);

            return response;
        }
    }
}
