using System;
using CodeBucket.Core.Data;
using System.Threading.Tasks;
using BitbucketSharp;
using BitbucketSharp.Models;

namespace CodeBucket.Core.Services
{
    public class LoginService : ILoginService
    {
        private readonly IAccountsService _accounts;

        public LoginService(IAccountsService accounts)
        {
            _accounts = accounts;
        }

		public async Task<Client> LoginAccount(BitbucketAccount account)
        {
            //Create the client
            UsersModel userInfo = null;
            var client = await Task.Run(() => Client.BearerLogin(account.Token, out userInfo));
            account.Username = userInfo.User.Username;
            account.AvatarUrl = userInfo.User.Avatar.Replace("/avatar/32", "/avatar/64");
            _accounts.Update(account);
            return client;
        }
    }
}
