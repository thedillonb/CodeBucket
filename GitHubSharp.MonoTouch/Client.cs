using System;
using System.Collections.Generic;
using System.Net;
using BitbucketSharp.Utils;
using RestSharp;
using RestSharp.Deserializers;

namespace GitHubSharp
{
    public class Client
    {
        private readonly RestClient _client = new RestClient("https://api.github.com");
        
        /// <summary>
        /// Gets the username for this clietn
        /// </summary>
        public String Username { get; private set; }
        
        public API API { get; private set; }
        
        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>
        /// The timeout.
        /// </value>
        public int Timeout 
        {
            get { return _client.Timeout; }
            set { _client.Timeout = value; }
        }
        
        public uint Retries { get; set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public Client(String username, String password)
        {
            Retries = 3;
            Username = username;
            API = new GitHubSharp.API(this);
            _client.Authenticator = new HttpBasicAuthenticator(username, password);
            _client.FollowRedirects = false;
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
        /// Makes a 'POST' request to the server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public T Post<T>(string uri, T data)
        {
            return Post<T>(uri, ObjectToDictionaryConverter.Convert(data));
        }
        
        /// <summary>
        /// Post the specified uri and data.
        /// </summary>
        public T Post<T, D>(string uri, D data)
        {
            return Post<T>(uri, ObjectToDictionaryConverter.Convert(data));
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
            Request(uri, Method.DELETE);
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
            
            RestSharp.IRestResponse response = null;
            for (var i = 0; i < Retries + 1; i++)
            {
                response = _client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    //A special case for deletes
                    if (request.Method == Method.DELETE && response.StatusCode == HttpStatusCode.NoContent)
                    {
                        //Do nothing. This is a special case...
                    }
                    else if (response.StatusCode == 0)
                    {
                        continue;
                    }
                    else
                    {
                        throw StatusCodeException.FactoryCreate(response.StatusCode);
                    }
                }
                
                //Return the response
                return response;
            }
            
            throw new InvalidOperationException("Unable to execute request. Status code 0 returned " + (Retries+1) + " times!");
        }
    }
    
    
}
