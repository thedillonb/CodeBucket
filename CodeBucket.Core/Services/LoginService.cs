using System;
using CodeFramework.Core.Services;
using CodeBucket.Core.Data;
using System.Threading.Tasks;
using BitbucketSharp;

namespace CodeBucket.Core.Services
{
    public class LoginService : ILoginService
    {
        private static readonly string[] Scopes = { "user", "public_repo", "repo", "notifications", "gist" };
        private readonly IAccountsService _accounts;

        public LoginService(IAccountsService accounts)
        {
            _accounts = accounts;
        }

		public async Task<LoginData> LoginWithToken(string clientId, string clientSecret, string code, string redirect, string requestDomain, string apiDomain, BitbucketAccount account)
        {
//			var token = await Task.Run(() => Client.RequestAccessToken(clientId, clientSecret, code, redirect, requestDomain));
//			var client = Client.BasicOAuth(token.AccessToken, apiDomain);
//			var info = await client.ExecuteAsync(client.AuthenticatedUser.GetInfo());
//            var username = info.Data.Login;
//
//            //Does this user exist?
//            var exists = account != null;
//			if (!exists)
//                account = new GitHubAccount { Username = username };
//			account.OAuth = token.AccessToken;
//            account.AvatarUrl = info.Data.AvatarUrl;
//			account.Domain = apiDomain;
//			account.WebDomain = requestDomain;
//			client.Username = username;
//
//            if (exists)
//                _accounts.Update(account);
//            else
//                _accounts.Insert(account);
//			return new LoginData { Client = client, Account = account };

			return null;
        }

		public async Task<Client> LoginAccount(BitbucketAccount account)
        {
            //Create the client
			Client client = null;
//			if (!string.IsNullOrEmpty(account.OAuth))
//			{
//				client = Client.BasicOAuth(account.OAuth, account.Domain ?? Client.DefaultApi);
//			}
//			else if (account.IsEnterprise || !string.IsNullOrEmpty(account.Password))
//			{
//				client = Client.Basic(account.Username, account.Password, account.Domain ?? Client.DefaultApi);
//			}
//
			client = new Client(account.Username, account.Password);


			var data = await Task.Run(() => client.Account.GetInfo(true));
			var userInfo = data.User;
			account.Username = userInfo.Username;
			account.AvatarUrl = userInfo.Avatar;
			client.Username = userInfo.Username;
            _accounts.Update(account);
            return client;
        }

		public LoginData Authenticate(string user, string pass, BitbucketAccount account)
        {
            try
            {
                //Make some valid checks
                if (string.IsNullOrEmpty(user))
                    throw new ArgumentException("Username is invalid");
                if (string.IsNullOrEmpty(pass))
                    throw new ArgumentException("Password is invalid");

                //Does this user exist?
                bool exists = account != null;
                if (!exists)
					account = new BitbucketAccount { Username = user };

				var client = new Client(user, pass);
				account.Password = pass;
				var data = client.Account.GetInfo(true);
				var userInfo = data.User;
				account.Username = userInfo.Username;
				account.AvatarUrl = userInfo.Avatar;
				client.Username = userInfo.Username;

                if (exists)
                    _accounts.Update(account);
                else
                    _accounts.Insert(account);

				return new LoginData { Client = client, Account = account };
            }
            catch (StatusCodeException ex)
            {
                throw new Exception("Unable to login as user " + user + ". Please check your credentials and try again.");
            }
        }
    }
}
